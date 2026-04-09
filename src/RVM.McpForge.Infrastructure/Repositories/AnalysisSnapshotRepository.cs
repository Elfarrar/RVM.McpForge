using Microsoft.EntityFrameworkCore;
using RVM.McpForge.Domain.Entities;
using RVM.McpForge.Domain.Interfaces;
using RVM.McpForge.Infrastructure.Data;

namespace RVM.McpForge.Infrastructure.Repositories;

public class AnalysisSnapshotRepository(McpForgeDbContext db) : IAnalysisSnapshotRepository
{
    public async Task<AnalysisSnapshot?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await db.AnalysisSnapshots
            .Include(x => x.Endpoints)
            .Include(x => x.Entities)
            .Include(x => x.Services)
            .Include(x => x.Tables).ThenInclude(t => t.Columns)
            .Include(x => x.Relationships)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<AnalysisSnapshot?> GetLatestByProjectIdAsync(Guid projectId, CancellationToken ct = default)
        => await db.AnalysisSnapshots
            .Include(x => x.Endpoints)
            .Include(x => x.Entities)
            .Include(x => x.Services)
            .Include(x => x.Tables).ThenInclude(t => t.Columns)
            .Include(x => x.Relationships)
            .Where(x => x.ForgeProjectId == projectId)
            .OrderByDescending(x => x.AnalyzedAt)
            .FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<AnalysisSnapshot>> GetByProjectIdAsync(Guid projectId, CancellationToken ct = default)
        => await db.AnalysisSnapshots
            .Where(x => x.ForgeProjectId == projectId)
            .OrderByDescending(x => x.AnalyzedAt)
            .ToListAsync(ct);

    public async Task AddAsync(AnalysisSnapshot snapshot, CancellationToken ct = default)
    {
        db.AnalysisSnapshots.Add(snapshot);
        await db.SaveChangesAsync(ct);
    }
}
