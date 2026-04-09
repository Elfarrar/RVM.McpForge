using RVM.McpForge.Domain.Enums;

namespace RVM.McpForge.Domain.Entities;

public class AnalysisSnapshot
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public Guid ForgeProjectId { get; set; }
    public SourceType SourceType { get; set; }
    public int ProjectCount { get; set; }
    public int TotalEndpoints { get; set; }
    public int TotalEntities { get; set; }
    public int TotalServices { get; set; }
    public int TotalTables { get; set; }
    public int TotalColumns { get; set; }
    public int TotalRelationships { get; set; }
    public DateTime AnalyzedAt { get; set; } = DateTime.UtcNow;

    public ForgeProject Project { get; set; } = null!;
    public ICollection<DiscoveredEndpoint> Endpoints { get; set; } = [];
    public ICollection<DiscoveredEntity> Entities { get; set; } = [];
    public ICollection<DiscoveredService> Services { get; set; } = [];
    public ICollection<DiscoveredTable> Tables { get; set; } = [];
    public ICollection<DiscoveredRelationship> Relationships { get; set; } = [];
}
