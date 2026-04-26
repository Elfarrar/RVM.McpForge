using RVM.McpForge.Application.Generators;
using RVM.McpForge.Domain.Enums;
using RVM.McpForge.Domain.Models;

namespace RVM.McpForge.Tests.Generators;

public class ScribanMcpGeneratorTests : IDisposable
{
    private readonly string _outputPath;
    private readonly ScribanMcpGenerator _generator = new();

    public ScribanMcpGeneratorTests()
    {
        _outputPath = Path.Combine(Path.GetTempPath(), $"mcpforge-test-{Guid.NewGuid().ToString()[..8]}");
    }

    public void Dispose()
    {
        if (Directory.Exists(_outputPath))
            Directory.Delete(_outputPath, recursive: true);
    }

    private static GenerationPlan MakePlan(string serverName, string outputPath, int toolCount = 2) =>
        new()
        {
            McpServerName = serverName,
            OutputPath = outputPath,
            Tools = Enumerable.Range(1, toolCount).Select(i => new McpToolDefinition
            {
                Name = $"query_table_{i}",
                Description = $"Query table {i}",
                Category = ToolCategory.Query
            }).ToList(),
            Resources = [new McpResourceDefinition { Uri = "db://schema/table", Name = "table", Description = "A table" }]
        };

    [Fact]
    public async Task GenerateAsync_CreatesOutputDirectory()
    {
        var plan = MakePlan("TestServer", _outputPath);

        await _generator.GenerateAsync(plan);

        Assert.True(Directory.Exists(_outputPath));
    }

    [Fact]
    public async Task GenerateAsync_CreatesCsprojFile()
    {
        var plan = MakePlan("MyMcpServer", _outputPath);

        await _generator.GenerateAsync(plan);

        var csproj = Path.Combine(_outputPath, "MyMcpServer.csproj");
        Assert.True(File.Exists(csproj));
    }

    [Fact]
    public async Task GenerateAsync_CreatesProgramCs()
    {
        var plan = MakePlan("MyMcpServer", _outputPath);

        await _generator.GenerateAsync(plan);

        var programFile = Path.Combine(_outputPath, "Program.cs");
        Assert.True(File.Exists(programFile));
    }

    [Fact]
    public async Task GenerateAsync_CreatesToolsDirectory()
    {
        var plan = MakePlan("MyMcpServer", _outputPath);

        await _generator.GenerateAsync(plan);

        var toolsDir = Path.Combine(_outputPath, "Tools");
        Assert.True(Directory.Exists(toolsDir));
    }

    [Fact]
    public async Task GenerateAsync_CreatesGeneratedToolsFile()
    {
        var plan = MakePlan("MyMcpServer", _outputPath);

        await _generator.GenerateAsync(plan);

        var generatedTools = Path.Combine(_outputPath, "Tools", "GeneratedTools.cs");
        Assert.True(File.Exists(generatedTools));
    }

    [Fact]
    public async Task GenerateAsync_ToolsFileContainsServerName()
    {
        var plan = MakePlan("AwesomeServer", _outputPath);

        await _generator.GenerateAsync(plan);

        var content = await File.ReadAllTextAsync(Path.Combine(_outputPath, "Tools", "GeneratedTools.cs"));
        Assert.Contains("AwesomeServer", content);
    }

    [Fact]
    public async Task GenerateAsync_ToolsFileContainsAllToolNames()
    {
        var plan = MakePlan("MyServer", _outputPath, toolCount: 3);

        await _generator.GenerateAsync(plan);

        var content = await File.ReadAllTextAsync(Path.Combine(_outputPath, "Tools", "GeneratedTools.cs"));
        Assert.Contains("query_table_1", content);
        Assert.Contains("query_table_2", content);
        Assert.Contains("query_table_3", content);
    }

    [Fact]
    public async Task GenerateAsync_CsprojContainsMcpPackageReference()
    {
        var plan = MakePlan("McpServer", _outputPath);

        await _generator.GenerateAsync(plan);

        var content = await File.ReadAllTextAsync(Path.Combine(_outputPath, "McpServer.csproj"));
        Assert.Contains("ModelContextProtocol", content);
    }

    [Fact]
    public async Task GenerateAsync_EmptyToolList_GeneratesEmptyToolsFile()
    {
        var plan = new GenerationPlan
        {
            McpServerName = "EmptyServer",
            OutputPath = _outputPath,
            Tools = [],
            Resources = []
        };

        await _generator.GenerateAsync(plan);

        var generatedTools = Path.Combine(_outputPath, "Tools", "GeneratedTools.cs");
        Assert.True(File.Exists(generatedTools));
    }

    [Fact]
    public async Task GenerateAsync_ToolNameWithUnderscores_ConvertsToPascalCase()
    {
        var plan = new GenerationPlan
        {
            McpServerName = "TestServer",
            OutputPath = _outputPath,
            Tools =
            [
                new McpToolDefinition { Name = "query_all_users", Description = "Query all users" }
            ]
        };

        await _generator.GenerateAsync(plan);

        var content = await File.ReadAllTextAsync(Path.Combine(_outputPath, "Tools", "GeneratedTools.cs"));
        // ToPascalCase("query_all_users") => "QueryAllUsersTool"
        Assert.Contains("QueryAllUsersTool", content);
    }

    [Fact]
    public async Task GenerateAsync_WithCancellationToken_CompletesOrCancels()
    {
        var plan = MakePlan("ServerX", _outputPath);
        using var cts = new CancellationTokenSource();

        // Should complete without cancellation
        await _generator.GenerateAsync(plan, cts.Token);

        Assert.True(File.Exists(Path.Combine(_outputPath, "ServerX.csproj")));
    }
}
