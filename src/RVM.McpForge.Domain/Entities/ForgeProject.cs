using RVM.McpForge.Domain.Enums;

namespace RVM.McpForge.Domain.Entities;

public class ForgeProject
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public string Name { get; set; } = string.Empty;
    public SourceType SourceType { get; set; }
    public string? RepositoryUrl { get; set; }
    public string? RepositoryPath { get; set; }
    public string? SolutionFile { get; set; }
    public string? ConnectionString { get; set; }
    public string? DatabaseName { get; set; }
    public string? Description { get; set; }
    public ForgeStatus Status { get; set; } = ForgeStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastAnalyzedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public ICollection<AnalysisSnapshot> Snapshots { get; set; } = [];
    public ICollection<GeneratedMcpProject> GeneratedProjects { get; set; } = [];
}
