using System;

namespace PhysicalStoragePurge.DTO;

public class PhysicalPurgeItem
{
    public long Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Path { get; set; } = string.Empty;

    public string Extension { get; set; } = string.Empty;

    public DateTime? DeletionDate { get; set; }

    // Storage id in the response
    public long StorageAttachmentId { get; set; }
}
