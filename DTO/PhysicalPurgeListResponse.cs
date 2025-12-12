using System.Collections.Generic;

namespace PhysicalStoragePurge.DTO;

public class PhysicalPurgeListResponse
{
    public int totalCount { get; set; }
    public int filteredCount { get; set; }
    public List<PhysicalPurgeItem> data { get; set; } = new();
}
