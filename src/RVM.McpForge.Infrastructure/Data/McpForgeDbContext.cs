using Microsoft.EntityFrameworkCore;
using RVM.McpForge.Domain.Entities;

namespace RVM.McpForge.Infrastructure.Data;

public class McpForgeDbContext(DbContextOptions<McpForgeDbContext> options) : DbContext(options)
{
    public DbSet<ForgeProject> ForgeProjects => Set<ForgeProject>();
    public DbSet<AnalysisSnapshot> AnalysisSnapshots => Set<AnalysisSnapshot>();
    public DbSet<DiscoveredTable> DiscoveredTables => Set<DiscoveredTable>();
    public DbSet<DiscoveredColumn> DiscoveredColumns => Set<DiscoveredColumn>();
    public DbSet<DiscoveredRelationship> DiscoveredRelationships => Set<DiscoveredRelationship>();
    public DbSet<DiscoveredEndpoint> DiscoveredEndpoints => Set<DiscoveredEndpoint>();
    public DbSet<DiscoveredEntity> DiscoveredEntities => Set<DiscoveredEntity>();
    public DbSet<DiscoveredService> DiscoveredServices => Set<DiscoveredService>();
    public DbSet<GeneratedMcpProject> GeneratedMcpProjects => Set<GeneratedMcpProject>();
    public DbSet<GeneratedTool> GeneratedTools => Set<GeneratedTool>();
    public DbSet<GeneratedResource> GeneratedResources => Set<GeneratedResource>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(McpForgeDbContext).Assembly);
    }
}
