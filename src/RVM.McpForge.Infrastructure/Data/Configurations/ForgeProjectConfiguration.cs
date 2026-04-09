using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RVM.McpForge.Domain.Entities;

namespace RVM.McpForge.Infrastructure.Data.Configurations;

public class ForgeProjectConfiguration : IEntityTypeConfiguration<ForgeProject>
{
    public void Configure(EntityTypeBuilder<ForgeProject> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.RepositoryUrl).HasMaxLength(500);
        builder.Property(x => x.RepositoryPath).HasMaxLength(500);
        builder.Property(x => x.SolutionFile).HasMaxLength(500);
        builder.Property(x => x.ConnectionString).HasMaxLength(1000);
        builder.Property(x => x.DatabaseName).HasMaxLength(200);
        builder.Property(x => x.Description).HasMaxLength(1000);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(50);
        builder.Property(x => x.SourceType).HasConversion<string>().HasMaxLength(50);

        builder.HasMany(x => x.Snapshots)
            .WithOne(x => x.Project)
            .HasForeignKey(x => x.ForgeProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.GeneratedProjects)
            .WithOne(x => x.Project)
            .HasForeignKey(x => x.ForgeProjectId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
