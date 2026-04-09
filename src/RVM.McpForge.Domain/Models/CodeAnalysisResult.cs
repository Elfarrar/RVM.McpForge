using RVM.McpForge.Domain.Entities;

namespace RVM.McpForge.Domain.Models;

public class CodeAnalysisResult
{
    public List<DiscoveredEndpoint> Endpoints { get; set; } = [];
    public List<DiscoveredEntity> Entities { get; set; } = [];
    public List<DiscoveredService> Services { get; set; } = [];
    public int ProjectCount { get; set; }
}
