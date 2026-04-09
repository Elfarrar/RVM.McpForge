using RVM.McpForge.Domain.Entities;

namespace RVM.McpForge.Domain.Models;

public class DatabaseAnalysisResult
{
    public List<DiscoveredTable> Tables { get; set; } = [];
    public List<DiscoveredColumn> Columns { get; set; } = [];
    public List<DiscoveredRelationship> Relationships { get; set; } = [];
}
