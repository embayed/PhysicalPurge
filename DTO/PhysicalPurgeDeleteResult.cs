namespace PhysicalStoragePurge.DTO;

public class PhysicalPurgeDeleteResult
{
    public bool Success { get; init; }

    public string? ErrorMessage { get; init; }

    public int StatusCode { get; init; }

    public long? FileId { get; init; }

    public long? StorageAttachmentId { get; init; }
}
