namespace RVM.McpForge.Domain.Models;

public class McpResourceDefinition
{
    public string Uri { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string MimeType { get; set; } = "application/json";
}
