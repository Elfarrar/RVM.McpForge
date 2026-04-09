using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RVM.McpForge.Domain.Entities;

namespace RVM.McpForge.Infrastructure.Data.Configurations;

public class DiscoveredRelationshipConfiguration : IEntityTypeConfiguration<DiscoveredRelationship>
{
    public void Configure(EntityTypeBuilder<DiscoveredRelationship> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ConstraintName).HasMaxLength(200).IsRequired();
        builder.Property(x => x.SourceTable).HasMaxLength(200).IsRequired();
        builder.Property(x => x.SourceColumn).HasMaxLength(200).IsRequired();
        builder.Property(x => x.TargetTable).HasMaxLength(200).IsRequired();
        builder.Property(x => x.TargetColumn).HasMaxLength(200).IsRequired();
        builder.Property(x => x.RelationType).HasMaxLength(20);
    }
}
