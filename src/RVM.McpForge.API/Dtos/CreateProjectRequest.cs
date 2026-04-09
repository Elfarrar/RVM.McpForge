using RVM.McpForge.Domain.Enums;

namespace RVM.McpForge.API.Dtos;

public record CreateProjectRequest(
    string Name,
    SourceType SourceType,
    string? RepositoryUrl,
    string? RepositoryPath,
    string? SolutionFile,
    string? ConnectionString,
    string? DatabaseName,
    string? Description);
