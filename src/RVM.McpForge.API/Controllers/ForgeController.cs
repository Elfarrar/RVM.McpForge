using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RVM.McpForge.API.Dtos;
using RVM.McpForge.Application.Services;
using RVM.McpForge.Domain.Entities;
using RVM.McpForge.Domain.Interfaces;

namespace RVM.McpForge.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ForgeController(
    IForgeProjectRepository projectRepo,
    IGeneratedMcpProjectRepository generatedRepo,
    ForgeOrchestrator orchestrator) : ControllerBase
{
    [HttpGet("projects")]
    public async Task<IActionResult> GetProjects(CancellationToken ct)
    {
        var projects = await projectRepo.GetAllAsync(ct);
        return Ok(projects);
    }

    [HttpGet("projects/{id:guid}")]
    public async Task<IActionResult> GetProject(Guid id, CancellationToken ct)
    {
        var project = await projectRepo.GetByIdAsync(id, ct);
        return project is null ? NotFound() : Ok(project);
    }

    [HttpPost("projects")]
    public async Task<IActionResult> CreateProject([FromBody] CreateProjectRequest request, CancellationToken ct)
    {
        var project = new ForgeProject
        {
            Name = request.Name,
            SourceType = request.SourceType,
            RepositoryUrl = request.RepositoryUrl,
            RepositoryPath = request.RepositoryPath,
            SolutionFile = request.SolutionFile,
            ConnectionString = request.ConnectionString,
            DatabaseName = request.DatabaseName,
            Description = request.Description
        };

        await projectRepo.AddAsync(project, ct);
        return CreatedAtAction(nameof(GetProject), new { id = project.Id }, project);
    }

    [HttpPost("projects/{id:guid}/analyze")]
    public async Task<IActionResult> Analyze(Guid id, CancellationToken ct)
    {
        var snapshot = await orchestrator.AnalyzeAsync(id, ct);
        return Ok(snapshot);
    }

    [HttpPost("projects/{id:guid}/generate")]
    public async Task<IActionResult> Generate(Guid id, [FromBody] GenerateRequest request, CancellationToken ct)
    {
        var generated = await orchestrator.GenerateAsync(id, request.ServerName, request.OutputPath, ct);
        return Ok(generated);
    }

    [HttpDelete("projects/{id:guid}")]
    public async Task<IActionResult> DeleteProject(Guid id, CancellationToken ct)
    {
        await projectRepo.DeleteAsync(id, ct);
        return NoContent();
    }

    [HttpGet("generated/{projectId:guid}")]
    public async Task<IActionResult> GetGenerated(Guid projectId, CancellationToken ct)
    {
        var generated = await generatedRepo.GetByProjectIdAsync(projectId, ct);
        return Ok(generated);
    }
}
