using RVM.McpForge.Domain.Enums;

namespace RVM.McpForge.Domain.Models;

public class McpToolDefinition
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ToolCategory Category { get; set; }
    public string InputSchemaJson { get; set; } = "{}";
    public string? SourceTable { get; set; }
    public string? SourceEndpoint { get; set; }
}
