namespace RVM.McpForge.Domain.Entities;

public class GeneratedMcpProject
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public Guid ForgeProjectId { get; set; }
    public Guid AnalysisSnapshotId { get; set; }
    public string OutputPath { get; set; } = string.Empty;
    public string McpServerName { get; set; } = string.Empty;
    public int ToolCount { get; set; }
    public int ResourceCount { get; set; }
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;

    public ForgeProject Project { get; set; } = null!;
    public AnalysisSnapshot Snapshot { get; set; } = null!;
    public ICollection<GeneratedTool> Tools { get; set; } = [];
    public ICollection<GeneratedResource> Resources { get; set; } = [];
}
