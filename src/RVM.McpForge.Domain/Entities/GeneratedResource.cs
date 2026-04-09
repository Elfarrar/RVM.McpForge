namespace RVM.McpForge.Domain.Entities;

public class GeneratedResource
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public Guid GeneratedMcpProjectId { get; set; }
    public string Uri { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string MimeType { get; set; } = "application/json";

    public GeneratedMcpProject GeneratedProject { get; set; } = null!;
}
