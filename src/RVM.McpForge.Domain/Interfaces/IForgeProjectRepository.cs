using RVM.McpForge.Domain.Entities;

namespace RVM.McpForge.Domain.Interfaces;

public interface IForgeProjectRepository
{
    Task<ForgeProject?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<ForgeProject>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(ForgeProject project, CancellationToken ct = default);
    Task UpdateAsync(ForgeProject project, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
