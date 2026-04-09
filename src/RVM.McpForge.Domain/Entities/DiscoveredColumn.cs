namespace RVM.McpForge.Domain.Entities;

public class DiscoveredColumn
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public Guid DiscoveredTableId { get; set; }
    public string ColumnName { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
    public bool IsNullable { get; set; }
    public bool IsPrimaryKey { get; set; }
    public string? DefaultValue { get; set; }
    public int? MaxLength { get; set; }
    public string? Comment { get; set; }

    public DiscoveredTable Table { get; set; } = null!;
}
