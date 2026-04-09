namespace RVM.McpForge.Domain.Entities;

public class DiscoveredRelationship
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public Guid AnalysisSnapshotId { get; set; }
    public string ConstraintName { get; set; } = string.Empty;
    public string SourceTable { get; set; } = string.Empty;
    public string SourceColumn { get; set; } = string.Empty;
    public string TargetTable { get; set; } = string.Empty;
    public string TargetColumn { get; set; } = string.Empty;
    public string RelationType { get; set; } = "FK";

    public AnalysisSnapshot Snapshot { get; set; } = null!;
}
