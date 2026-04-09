using RVM.McpForge.Domain.Entities;

namespace RVM.McpForge.Domain.Interfaces;

public interface IGeneratedMcpProjectRepository
{
    Task<GeneratedMcpProject?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<GeneratedMcpProject>> GetByProjectIdAsync(Guid projectId, CancellationToken ct = default);
    Task AddAsync(GeneratedMcpProject generated, CancellationToken ct = default);
}
