using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RVM.McpForge.Domain.Entities;

namespace RVM.McpForge.Infrastructure.Data.Configurations;

public class DiscoveredEntityConfiguration : IEntityTypeConfiguration<DiscoveredEntity>
{
    public void Configure(EntityTypeBuilder<DiscoveredEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Namespace).HasMaxLength(500).IsRequired();
        builder.Property(x => x.BaseType).HasMaxLength(200);
        builder.Property(x => x.PropertiesJson).HasColumnType("jsonb");
    }
}
