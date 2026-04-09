using RVM.McpForge.Domain.Entities;

namespace RVM.McpForge.Application.Analyzers;

public interface ISourceAnalyzer
{
    Task<AnalysisSnapshot> AnalyzeAsync(ForgeProject project, CancellationToken ct = default);
}
