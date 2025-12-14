using System.Linq;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using PhysicalStoragePurge.Data;
using PhysicalStoragePurge.Entities;

namespace PhysicalStoragePurge.Repositories;

public class FileRepository : IFileRepository
{
    private readonly DmsDbContext _dbContext;

    public FileRepository(DmsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> CountPendingPhysicalPurgeAsync(bool hasDeletionDate, CancellationToken cancellationToken)
    {
        var query = BuildPendingPurgeQuery(hasDeletionDate);
        return await query.CountAsync(cancellationToken);
    }

    public async Task<List<FileEntity>> GetPendingPhysicalPurgeAsync(int page, int pageSize, bool hasDeletionDate, CancellationToken cancellationToken)
    {
        var query = BuildPendingPurgeQuery(hasDeletionDate);

        return await query
            .OrderBy(f => f.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public Task<FileEntity?> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        return _dbContext.Files.FirstOrDefaultAsync(f => f.Id == id, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }

    private IQueryable<FileEntity> BuildPendingPurgeQuery(bool hasDeletionDate)
    {
        var query = _dbContext.Files
            .Where(f => f.IsDeleted && !f.IsPhysicallyPurged);

        if (hasDeletionDate)
        {
            query = query.Where(f => f.DeletionDate != null);
        }

        return query;
    }
}
