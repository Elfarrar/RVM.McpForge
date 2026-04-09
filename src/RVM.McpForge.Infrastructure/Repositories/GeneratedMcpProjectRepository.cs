using Microsoft.EntityFrameworkCore;
using RVM.McpForge.Domain.Entities;
using RVM.McpForge.Domain.Interfaces;
using RVM.McpForge.Infrastructure.Data;

namespace RVM.McpForge.Infrastructure.Repositories;

public class GeneratedMcpProjectRepository(McpForgeDbContext db) : IGeneratedMcpProjectRepository
{
    public async Task<GeneratedMcpProject?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await db.GeneratedMcpProjects
            .Include(x => x.Tools)
            .Include(x => x.Resources)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<IReadOnlyList<GeneratedMcpProject>> GetByProjectIdAsync(Guid projectId, CancellationToken ct = default)
        => await db.GeneratedMcpProjects
            .Where(x => x.ForgeProjectId == projectId)
            .OrderByDescending(x => x.GeneratedAt)
            .ToListAsync(ct);

    public async Task AddAsync(GeneratedMcpProject generated, CancellationToken ct = default)
    {
        db.GeneratedMcpProjects.Add(generated);
        await db.SaveChangesAsync(ct);
    }
}
