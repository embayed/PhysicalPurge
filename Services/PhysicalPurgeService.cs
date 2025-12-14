using System.Linq;
using System.Threading;
using Intalio.Storage.FileSystem.Core.API;
using Intalio.Storage.Interface.Model;
using Microsoft.AspNetCore.Http;
using PhysicalStoragePurge.DTO;
using PhysicalStoragePurge.Repositories;

namespace PhysicalStoragePurge.Services;

public class PhysicalPurgeService : IPhysicalPurgeService
{
    private readonly IFileRepository _fileRepository;
    private readonly ManageStorage _manageStorage;

    public PhysicalPurgeService(IFileRepository fileRepository, ManageStorage manageStorage)
    {
        _fileRepository = fileRepository;
        _manageStorage = manageStorage;
    }

    public async Task<PhysicalPurgeListResponse> GetPendingPurgesAsync(int page, int pageSize, bool hasDeletionDate, CancellationToken cancellationToken)
    {
        if (page < 1) page = 1;
        if (pageSize <= 0) pageSize = 10;

        var total = await _fileRepository.CountPendingPhysicalPurgeAsync(hasDeletionDate, cancellationToken);
        var files = await _fileRepository.GetPendingPhysicalPurgeAsync(page, pageSize, hasDeletionDate, cancellationToken);

        var items = files.Select(f => new PhysicalPurgeItem
        {
            Id = f.Id,
            Name = f.Name,
            Path = f.Path,
            Extension = f.Extension,
            DeletionDate = f.DeletionDate,
            StorageAttachmentId = f.StorageAttachmentId
        }).ToList();

        return new PhysicalPurgeListResponse
        {
            totalCount = total,
            filteredCount = total,
            data = items
        };
    }

    public async Task<PhysicalPurgeDeleteResult> DeleteAsync(long fileId, long storageAttachmentId, CancellationToken cancellationToken)
    {
        var file = await _fileRepository.GetByIdAsync(fileId, cancellationToken);
        if (file == null)
        {
            return new PhysicalPurgeDeleteResult
            {
                Success = false,
                StatusCode = StatusCodes.Status404NotFound,
                ErrorMessage = "File not found with the given fileId."
            };
        }

        var logicalDeleteResult = await _manageStorage.DeleteFile(storageAttachmentId);
        if (!logicalDeleteResult)
        {
            return new PhysicalPurgeDeleteResult
            {
                Success = false,
                StatusCode = StatusCodes.Status500InternalServerError,
                ErrorMessage = "Logical delete failed after database lookup.",
                FileId = fileId,
                StorageAttachmentId = storageAttachmentId
            };
        }

        var recycleItem = new RecycleBinItemModel
        {
            Id = storageAttachmentId,
            Type = ItemType.Attachment
        };

        var physicalDeleteResult = await _manageStorage.DeleteItem(recycleItem);
        if (!physicalDeleteResult)
        {
            return new PhysicalPurgeDeleteResult
            {
                Success = false,
                StatusCode = StatusCodes.Status500InternalServerError,
                ErrorMessage = "Physical delete failed after logical deletion.",
                FileId = fileId,
                StorageAttachmentId = storageAttachmentId
            };
        }

        file.IsPhysicallyPurged = true;
        await _fileRepository.SaveChangesAsync(cancellationToken);

        return new PhysicalPurgeDeleteResult
        {
            Success = true,
            StatusCode = StatusCodes.Status200OK,
            FileId = fileId,
            StorageAttachmentId = storageAttachmentId
        };
    }
}
