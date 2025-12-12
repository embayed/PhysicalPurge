using System;

namespace PhysicalStoragePurge.Entities;

public class FileEntity
{
    public long Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Path { get; set; } = string.Empty;

    public string Extension { get; set; } = string.Empty;

    public bool IsDeleted { get; set; }

    public DateTime? DeletionDate { get; set; }

    // NEW: storage id from "StorageAttachmentId" column
    public long StorageAttachmentId { get; set; }

    public bool IsPhysicallyPurged { get; set; }
}
