using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RVM.McpForge.Domain.Entities;

namespace RVM.McpForge.Infrastructure.Data.Configurations;

public class DiscoveredServiceConfiguration : IEntityTypeConfiguration<DiscoveredService>
{
    public void Configure(EntityTypeBuilder<DiscoveredService> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Namespace).HasMaxLength(500).IsRequired();
        builder.Property(x => x.MethodsJson).HasColumnType("jsonb");
    }
}
