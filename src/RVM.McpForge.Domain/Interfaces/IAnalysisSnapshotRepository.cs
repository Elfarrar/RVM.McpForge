using RVM.McpForge.Domain.Entities;

namespace RVM.McpForge.Domain.Interfaces;

public interface IAnalysisSnapshotRepository
{
    Task<AnalysisSnapshot?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<AnalysisSnapshot?> GetLatestByProjectIdAsync(Guid projectId, CancellationToken ct = default);
    Task<IReadOnlyList<AnalysisSnapshot>> GetByProjectIdAsync(Guid projectId, CancellationToken ct = default);
    Task AddAsync(AnalysisSnapshot snapshot, CancellationToken ct = default);
}
