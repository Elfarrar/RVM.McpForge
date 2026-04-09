namespace RVM.McpForge.Domain.Entities;

public class DiscoveredEndpoint
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public Guid AnalysisSnapshotId { get; set; }
    public string HttpMethod { get; set; } = string.Empty;
    public string Route { get; set; } = string.Empty;
    public string ControllerName { get; set; } = string.Empty;
    public string ActionName { get; set; } = string.Empty;
    public string? ReturnType { get; set; }
    public string? RequestBodyType { get; set; }

    public AnalysisSnapshot Snapshot { get; set; } = null!;
}
