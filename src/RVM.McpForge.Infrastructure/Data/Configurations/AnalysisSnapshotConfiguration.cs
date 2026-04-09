using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RVM.McpForge.Domain.Entities;

namespace RVM.McpForge.Infrastructure.Data.Configurations;

public class AnalysisSnapshotConfiguration : IEntityTypeConfiguration<AnalysisSnapshot>
{
    public void Configure(EntityTypeBuilder<AnalysisSnapshot> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.SourceType).HasConversion<string>().HasMaxLength(50);

        builder.HasMany(x => x.Endpoints)
            .WithOne(x => x.Snapshot)
            .HasForeignKey(x => x.AnalysisSnapshotId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Entities)
            .WithOne(x => x.Snapshot)
            .HasForeignKey(x => x.AnalysisSnapshotId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Services)
            .WithOne(x => x.Snapshot)
            .HasForeignKey(x => x.AnalysisSnapshotId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Tables)
            .WithOne(x => x.Snapshot)
            .HasForeignKey(x => x.AnalysisSnapshotId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Relationships)
            .WithOne(x => x.Snapshot)
            .HasForeignKey(x => x.AnalysisSnapshotId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
