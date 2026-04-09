using RVM.McpForge.Domain.Enums;

namespace RVM.McpForge.Domain.Entities;

public class GeneratedTool
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public Guid GeneratedMcpProjectId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ToolCategory Category { get; set; }
    public string InputSchemaJson { get; set; } = "{}";
    public string? SourceTable { get; set; }
    public string? SourceEndpoint { get; set; }

    public GeneratedMcpProject GeneratedProject { get; set; } = null!;
}
