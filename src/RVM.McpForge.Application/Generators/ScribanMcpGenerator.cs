using System.Text.RegularExpressions;
using RVM.McpForge.Domain.Models;
using Scriban;

namespace RVM.McpForge.Application.Generators;

public partial class ScribanMcpGenerator : IMcpProjectGenerator
{
    public async Task GenerateAsync(GenerationPlan plan, CancellationToken ct = default)
    {
        Directory.CreateDirectory(plan.OutputPath);
        var toolsDir = Path.Combine(plan.OutputPath, "Tools");
        Directory.CreateDirectory(toolsDir);

        await GenerateCsprojAsync(plan, ct);
        await GenerateProgramAsync(plan, ct);
        await GenerateToolsAsync(plan, toolsDir, ct);
    }

    private static async Task GenerateCsprojAsync(GenerationPlan plan, CancellationToken ct)
    {
        var templateText = await LoadTemplateAsync("McpCsprojTemplate.sbn", ct);
        var template = Template.Parse(templateText);
        var result = template.Render(new { server_name = plan.McpServerName });
        await File.WriteAllTextAsync(Path.Combine(plan.OutputPath, $"{plan.McpServerName}.csproj"), result, ct);
    }

    private static async Task GenerateProgramAsync(GenerationPlan plan, CancellationToken ct)
    {
        var templateText = await LoadTemplateAsync("McpProgramTemplate.sbn", ct);
        var template = Template.Parse(templateText);
        var result = template.Render();
        await File.WriteAllTextAsync(Path.Combine(plan.OutputPath, "Program.cs"), result, ct);
    }

    private async Task GenerateToolsAsync(GenerationPlan plan, string toolsDir, CancellationToken ct)
    {
        var templateText = await LoadTemplateAsync("McpServerTemplate.sbn", ct);
        var template = Template.Parse(templateText);

        var toolModels = plan.Tools.Select(t => new
        {
            name = t.Name,
            class_name = ToPascalCase(t.Name) + "Tool",
            method_name = ToPascalCase(t.Name),
            description = t.Description.Replace("\"", "\\\""),
            parameters = ""
        }).ToList();

        var result = template.Render(new { server_name = plan.McpServerName, tools = toolModels });
        await File.WriteAllTextAsync(Path.Combine(toolsDir, "GeneratedTools.cs"), result, ct);
    }

    private static async Task<string> LoadTemplateAsync(string templateName, CancellationToken ct)
    {
        var assemblyDir = Path.GetDirectoryName(typeof(ScribanMcpGenerator).Assembly.Location)!;
        var templatePath = Path.Combine(assemblyDir, "Generators", "Templates", templateName);

        if (!File.Exists(templatePath))
        {
            var srcDir = AppContext.BaseDirectory;
            templatePath = Path.Combine(srcDir, "Generators", "Templates", templateName);
        }

        return await File.ReadAllTextAsync(templatePath, ct);
    }

    private static string ToPascalCase(string input)
    {
        return string.Concat(
            PascalSplitRegex().Split(input)
                .Where(s => !string.IsNullOrEmpty(s))
                .Select(s => char.ToUpperInvariant(s[0]) + s[1..].ToLowerInvariant()));
    }

    [GeneratedRegex(@"[_\-\s]+")]
    private static partial Regex PascalSplitRegex();
}
