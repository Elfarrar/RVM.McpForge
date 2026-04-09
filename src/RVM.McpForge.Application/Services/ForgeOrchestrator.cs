using RVM.McpForge.Application.Analyzers;
using RVM.McpForge.Application.Analyzers.Roslyn;
using RVM.McpForge.Application.Generators;
using RVM.McpForge.Application.Planners;
using RVM.McpForge.Domain.Entities;
using RVM.McpForge.Domain.Enums;
using RVM.McpForge.Domain.Interfaces;

namespace RVM.McpForge.Application.Services;

public class ForgeOrchestrator(
    IForgeProjectRepository projectRepo,
    IAnalysisSnapshotRepository snapshotRepo,
    IGeneratedMcpProjectRepository generatedRepo,
    RoslynAnalyzer roslynAnalyzer,
    DatabaseAnalyzer databaseAnalyzer,
    GitCloneService cloneService,
    IToolPlanner planner,
    IMcpProjectGenerator generator)
{
    public async Task<AnalysisSnapshot> AnalyzeAsync(Guid projectId, CancellationToken ct = default)
    {
        var project = await projectRepo.GetByIdAsync(projectId, ct)
            ?? throw new InvalidOperationException($"Project {projectId} not found.");

        // Auto-clone if RepositoryUrl is set but no local path exists
        if (project.SourceType == SourceType.Git
            && !string.IsNullOrWhiteSpace(project.RepositoryUrl)
            && string.IsNullOrWhiteSpace(project.RepositoryPath))
        {
            project.RepositoryPath = cloneService.Clone(project.RepositoryUrl);
            await projectRepo.UpdateAsync(project, ct);
        }

        project.Status = ForgeStatus.Analyzing;
        await projectRepo.UpdateAsync(project, ct);

        try
        {
            ISourceAnalyzer analyzer = project.SourceType switch
            {
                SourceType.Git => roslynAnalyzer,
                SourceType.Database => databaseAnalyzer,
                _ => throw new NotSupportedException($"SourceType {project.SourceType} not supported.")
            };

            var snapshot = await analyzer.AnalyzeAsync(project, ct);
            await snapshotRepo.AddAsync(snapshot, ct);

            project.Status = ForgeStatus.Analyzed;
            project.LastAnalyzedAt = DateTime.UtcNow;
            await projectRepo.UpdateAsync(project, ct);

            return snapshot;
        }
        catch
        {
            project.Status = ForgeStatus.Failed;
            await projectRepo.UpdateAsync(project, ct);
            throw;
        }
    }

    public async Task<GeneratedMcpProject> GenerateAsync(Guid projectId, string serverName, string outputPath, CancellationToken ct = default)
    {
        var project = await projectRepo.GetByIdAsync(projectId, ct)
            ?? throw new InvalidOperationException($"Project {projectId} not found.");

        var snapshot = await snapshotRepo.GetLatestByProjectIdAsync(projectId, ct)
            ?? throw new InvalidOperationException($"No analysis snapshot found for project {projectId}. Run analysis first.");

        project.Status = ForgeStatus.Generating;
        await projectRepo.UpdateAsync(project, ct);

        try
        {
            var plan = planner.CreatePlan(snapshot, serverName, outputPath);
            await generator.GenerateAsync(plan, ct);

            var generated = new GeneratedMcpProject
            {
                ForgeProjectId = projectId,
                AnalysisSnapshotId = snapshot.Id,
                OutputPath = outputPath,
                McpServerName = serverName,
                ToolCount = plan.Tools.Count,
                ResourceCount = plan.Resources.Count
            };

            await generatedRepo.AddAsync(generated, ct);

            project.Status = ForgeStatus.Ready;
            await projectRepo.UpdateAsync(project, ct);

            return generated;
        }
        catch
        {
            project.Status = ForgeStatus.Failed;
            await projectRepo.UpdateAsync(project, ct);
            throw;
        }
    }
}
