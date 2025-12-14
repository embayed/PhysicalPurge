using System.Threading;
using PhysicalStoragePurge.Entities;

namespace PhysicalStoragePurge.Repositories;

public interface IFileRepository
{
    Task<int> CountPendingPhysicalPurgeAsync(bool hasDeletionDate, CancellationToken cancellationToken);

    Task<List<FileEntity>> GetPendingPhysicalPurgeAsync(int page, int pageSize, bool hasDeletionDate, CancellationToken cancellationToken);

    Task<FileEntity?> GetByIdAsync(long id, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
