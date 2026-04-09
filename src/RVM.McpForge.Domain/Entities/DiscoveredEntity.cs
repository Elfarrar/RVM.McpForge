namespace RVM.McpForge.Domain.Entities;

public class DiscoveredEntity
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public Guid AnalysisSnapshotId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Namespace { get; set; } = string.Empty;
    public string? BaseType { get; set; }
    public string PropertiesJson { get; set; } = "[]";

    public AnalysisSnapshot Snapshot { get; set; } = null!;
}
