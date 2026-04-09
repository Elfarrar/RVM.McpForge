namespace RVM.McpForge.Domain.Entities;

public class DiscoveredTable
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public Guid AnalysisSnapshotId { get; set; }
    public string SchemaName { get; set; } = "public";
    public string TableName { get; set; } = string.Empty;
    public string? Comment { get; set; }

    public AnalysisSnapshot Snapshot { get; set; } = null!;
    public ICollection<DiscoveredColumn> Columns { get; set; } = [];
}
