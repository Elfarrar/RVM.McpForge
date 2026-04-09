using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RVM.McpForge.Domain.Entities;

namespace RVM.McpForge.Infrastructure.Data.Configurations;

public class GeneratedMcpProjectConfiguration : IEntityTypeConfiguration<GeneratedMcpProject>
{
    public void Configure(EntityTypeBuilder<GeneratedMcpProject> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.OutputPath).HasMaxLength(500).IsRequired();
        builder.Property(x => x.McpServerName).HasMaxLength(200).IsRequired();

        builder.HasOne(x => x.Snapshot)
            .WithMany()
            .HasForeignKey(x => x.AnalysisSnapshotId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Tools)
            .WithOne(x => x.GeneratedProject)
            .HasForeignKey(x => x.GeneratedMcpProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Resources)
            .WithOne(x => x.GeneratedProject)
            .HasForeignKey(x => x.GeneratedMcpProjectId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
