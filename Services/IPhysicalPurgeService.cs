using System.Threading;
using PhysicalStoragePurge.DTO;

namespace PhysicalStoragePurge.Services;

public interface IPhysicalPurgeService
{
    Task<PhysicalPurgeListResponse> GetPendingPurgesAsync(int page, int pageSize, bool hasDeletionDate, CancellationToken cancellationToken);

    Task<PhysicalPurgeDeleteResult> DeleteAsync(long fileId, long storageAttachmentId, CancellationToken cancellationToken);
}
