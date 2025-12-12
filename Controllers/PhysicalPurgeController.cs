using Intalio.Storage.FileSystem.Core.API;
using Intalio.Storage.Interface;
using Intalio.Storage.Interface.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhysicalStoragePurge.Data;
using PhysicalStoragePurge.DTO;

namespace PhysicalStoragePurge.Controllers;

[Route("[controller]")]
[ApiController]
public class PhysicalPurgeController : ControllerBase
{
    private readonly DmsDbContext _db;

    public PhysicalPurgeController(DmsDbContext db)
    {
        _db = db;
    }

    // GET /PhysicalPurge/List?page=1&pageSize=10&hasDeletionDate=true
    [HttpGet("List")]
    public async Task<IActionResult> List(
        int page = 1,
        int pageSize = 10,
        bool hasDeletionDate = true)
    {
        if (page < 1) page = 1;
        if (pageSize <= 0) pageSize = 10;

        // base query: only deleted files
        var query = _db.Files
            .Where(f => f.IsDeleted && !f.IsPhysicallyPurged);

        // optional filter: only rows with DeletionDate
        if (hasDeletionDate)
        {
            query = query.Where(f => f.DeletionDate != null);
        }

        var total = await query.CountAsync();

        var items = await query
            .OrderBy(f => f.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(f => new PhysicalPurgeItem
            {
                Id = f.Id,
                Name = f.Name,
                Path = f.Path,
                Extension = f.Extension,
                DeletionDate = f.DeletionDate,
                StorageAttachmentId = f.StorageAttachmentId
            })
            .ToListAsync();

        var response = new PhysicalPurgeListResponse
        {
            totalCount = total,
            filteredCount = total,
            data = items
        };

        return Ok(response);
    }

    // DELETE /PhysicalPurge/Delete/{id}
    [HttpDelete("Delete/{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        // Step 0: update File table first
        var file = await _db.Files.FirstOrDefaultAsync(f => f.Id == id);
        if (file == null)
        {
            return NotFound(new { message = "File not found." });
        }

        file.IsPhysicallyPurged = true;
        await _db.SaveChangesAsync();

        var storage = new ManageStorage();

        // Step 1: logical delete (move to recycle bin)
        var logicalDeleteResult = await storage.DeleteFile(id);
        if (!logicalDeleteResult)
        {
            return StatusCode(500, new { message = "Logical delete failed after database update." });
        }

        // Step 2: physical delete
        var recycleItem = new RecycleBinItemModel
        {
            Id = id,
            Type = ItemType.Attachment
        };

        var physicalDeleteResult = await storage.DeleteItem(recycleItem);
        if (!physicalDeleteResult)
        {
            return StatusCode(500, new { message = "Physical delete failed after database update." });
        }

        return Ok(new
        {
            id,
            physicallyPurged = true
        });
    }

}
