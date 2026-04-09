using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RVM.McpForge.Domain.Entities;

namespace RVM.McpForge.Infrastructure.Data.Configurations;

public class DiscoveredTableConfiguration : IEntityTypeConfiguration<DiscoveredTable>
{
    public void Configure(EntityTypeBuilder<DiscoveredTable> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.TableName).HasMaxLength(200).IsRequired();
        builder.Property(x => x.SchemaName).HasMaxLength(100);
        builder.Property(x => x.Comment).HasMaxLength(1000);

        builder.HasMany(x => x.Columns)
            .WithOne(x => x.Table)
            .HasForeignKey(x => x.DiscoveredTableId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
