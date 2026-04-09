using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RVM.McpForge.Domain.Entities;

namespace RVM.McpForge.Infrastructure.Data.Configurations;

public class DiscoveredColumnConfiguration : IEntityTypeConfiguration<DiscoveredColumn>
{
    public void Configure(EntityTypeBuilder<DiscoveredColumn> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ColumnName).HasMaxLength(200).IsRequired();
        builder.Property(x => x.DataType).HasMaxLength(100).IsRequired();
        builder.Property(x => x.DefaultValue).HasMaxLength(500);
        builder.Property(x => x.Comment).HasMaxLength(1000);
    }
}
