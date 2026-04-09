using RVM.McpForge.Domain.Models;

namespace RVM.McpForge.Application.Generators;

public interface IMcpProjectGenerator
{
    Task GenerateAsync(GenerationPlan plan, CancellationToken ct = default);
}
