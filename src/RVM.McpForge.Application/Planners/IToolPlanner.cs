using RVM.McpForge.Domain.Entities;
using RVM.McpForge.Domain.Models;

namespace RVM.McpForge.Application.Planners;

public interface IToolPlanner
{
    GenerationPlan CreatePlan(AnalysisSnapshot snapshot, string serverName, string outputPath);
}
