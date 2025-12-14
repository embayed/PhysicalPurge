using System.Threading;
using Microsoft.AspNetCore.Mvc;
using PhysicalStoragePurge.DTO;
using PhysicalStoragePurge.Services;

namespace PhysicalStoragePurge.Controllers;

[Route("[controller]")]
[ApiController]
public class PhysicalPurgeController : ControllerBase
{
    private readonly IPhysicalPurgeService _physicalPurgeService;

    public PhysicalPurgeController(IPhysicalPurgeService physicalPurgeService)
    {
        _physicalPurgeService = physicalPurgeService;
    }

    // GET /PhysicalPurge/List?page=1&pageSize=10&hasDeletionDate=true
    [HttpGet("List")]
    public async Task<IActionResult> List(
        int page = 1,
        int pageSize = 10,
        bool hasDeletionDate = true,
        CancellationToken cancellationToken = default)
    {
        var response = await _physicalPurgeService.GetPendingPurgesAsync(page, pageSize, hasDeletionDate, cancellationToken);

        return Ok(response);
    }

    // DELETE /PhysicalPurge/Delete/{fileId}/{storageAttachmentId}
    [HttpDelete("Delete/{fileId:long}/{storageAttachmentId:long}")]
    public async Task<IActionResult> Delete(long fileId, long storageAttachmentId, CancellationToken cancellationToken = default)
    {
        var result = await _physicalPurgeService.DeleteAsync(fileId, storageAttachmentId, cancellationToken);

        if (!result.Success)
        {
            return StatusCode(result.StatusCode, new
            {
                message = result.ErrorMessage,
                result.FileId,
                result.StorageAttachmentId
            });
        }

        return Ok(new
        {
            result.FileId,
            result.StorageAttachmentId,
            physicallyPurged = true
        });
    }
}
