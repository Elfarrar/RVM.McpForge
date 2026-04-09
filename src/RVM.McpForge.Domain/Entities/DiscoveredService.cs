namespace RVM.McpForge.Domain.Entities;

public class DiscoveredService
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public Guid AnalysisSnapshotId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Namespace { get; set; } = string.Empty;
    public bool IsInterface { get; set; }
    public string MethodsJson { get; set; } = "[]";

    public AnalysisSnapshot Snapshot { get; set; } = null!;
}
