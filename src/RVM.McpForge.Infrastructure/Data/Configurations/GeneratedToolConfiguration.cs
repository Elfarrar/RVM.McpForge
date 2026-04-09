using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RVM.McpForge.Domain.Entities;

namespace RVM.McpForge.Infrastructure.Data.Configurations;

public class GeneratedToolConfiguration : IEntityTypeConfiguration<GeneratedTool>
{
    public void Configure(EntityTypeBuilder<GeneratedTool> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(1000).IsRequired();
        builder.Property(x => x.Category).HasConversion<string>().HasMaxLength(50);
        builder.Property(x => x.InputSchemaJson).HasColumnType("jsonb");
        builder.Property(x => x.SourceTable).HasMaxLength(200);
        builder.Property(x => x.SourceEndpoint).HasMaxLength(500);
    }
}
