namespace RVM.McpForge.Domain.Models;

public class GenerationPlan
{
    public string McpServerName { get; set; } = string.Empty;
    public string OutputPath { get; set; } = string.Empty;
    public List<McpToolDefinition> Tools { get; set; } = [];
    public List<McpResourceDefinition> Resources { get; set; } = [];
}
