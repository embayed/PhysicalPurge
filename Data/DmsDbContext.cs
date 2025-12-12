using Microsoft.EntityFrameworkCore;
using PhysicalStoragePurge.Entities;

namespace PhysicalStoragePurge.Data;

public class DmsDbContext : DbContext
{
    public DmsDbContext(DbContextOptions<DmsDbContext> options)
        : base(options)
    {
    }

    // DbSet for the File table
    public DbSet<FileEntity> Files => Set<FileEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var file = modelBuilder.Entity<FileEntity>();

        // Map to public."File"
        file.ToTable("File", "public");

        file.HasKey(f => f.Id);

        file.Property(f => f.Id).HasColumnName("Id");
        file.Property(f => f.Name).HasColumnName("Name");
        file.Property(f => f.Path).HasColumnName("Path");
        file.Property(f => f.Extension).HasColumnName("Extension");
        file.Property(f => f.IsDeleted).HasColumnName("IsDeleted");
        file.Property(f => f.DeletionDate).HasColumnName("DeletionDate");
        file.Property(f => f.StorageAttachmentId).HasColumnName("StorageAttachmentId");
        file.Property(f => f.IsPhysicallyPurged).HasColumnName("IsPhysicallyPurged");
    }
}
