using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RVM.McpForge.Domain.Entities;

namespace RVM.McpForge.Infrastructure.Data.Configurations;

public class DiscoveredEndpointConfiguration : IEntityTypeConfiguration<DiscoveredEndpoint>
{
    public void Configure(EntityTypeBuilder<DiscoveredEndpoint> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.HttpMethod).HasMaxLength(10).IsRequired();
        builder.Property(x => x.Route).HasMaxLength(500).IsRequired();
        builder.Property(x => x.ControllerName).HasMaxLength(200).IsRequired();
        builder.Property(x => x.ActionName).HasMaxLength(200).IsRequired();
        builder.Property(x => x.ReturnType).HasMaxLength(200);
        builder.Property(x => x.RequestBodyType).HasMaxLength(200);
    }
}
