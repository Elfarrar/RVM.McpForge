using Microsoft.EntityFrameworkCore;
using RVM.McpForge.Domain.Entities;
using RVM.McpForge.Domain.Interfaces;
using RVM.McpForge.Infrastructure.Data;

namespace RVM.McpForge.Infrastructure.Repositories;

public class ForgeProjectRepository(McpForgeDbContext db) : IForgeProjectRepository
{
    public async Task<ForgeProject?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await db.ForgeProjects
            .Include(x => x.Snapshots)
            .Include(x => x.GeneratedProjects)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<IReadOnlyList<ForgeProject>> GetAllAsync(CancellationToken ct = default)
        => await db.ForgeProjects
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(ct);

    public async Task AddAsync(ForgeProject project, CancellationToken ct = default)
    {
        db.ForgeProjects.Add(project);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(ForgeProject project, CancellationToken ct = default)
    {
        db.ForgeProjects.Update(project);
        await db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var project = await db.ForgeProjects.FindAsync([id], ct);
        if (project is not null)
        {
            db.ForgeProjects.Remove(project);
            await db.SaveChangesAsync(ct);
        }
    }
}
