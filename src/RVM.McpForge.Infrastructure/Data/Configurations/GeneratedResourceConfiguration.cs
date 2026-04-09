using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RVM.McpForge.Domain.Entities;

namespace RVM.McpForge.Infrastructure.Data.Configurations;

public class GeneratedResourceConfiguration : IEntityTypeConfiguration<GeneratedResource>
{
    public void Configure(EntityTypeBuilder<GeneratedResource> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Uri).HasMaxLength(500).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(1000).IsRequired();
        builder.Property(x => x.MimeType).HasMaxLength(100).IsRequired();
    }
}
