using RVM.McpForge.Domain.Entities;
using RVM.McpForge.Domain.Enums;

namespace RVM.McpForge.Tests;

public class ForgeProjectTests
{
    [Fact]
    public void NewProject_HasDefaultValues()
    {
        var project = new ForgeProject();

        Assert.NotEqual(Guid.Empty, project.Id);
        Assert.Equal(ForgeStatus.Pending, project.Status);
        Assert.Equal(string.Empty, project.Name);
        Assert.True(project.CreatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void AnalysisSnapshot_DefaultCounts_AreZero()
    {
        var snapshot = new AnalysisSnapshot();

        Assert.Equal(0, snapshot.ProjectCount);
        Assert.Equal(0, snapshot.TotalEndpoints);
        Assert.Equal(0, snapshot.TotalEntities);
        Assert.Equal(0, snapshot.TotalTables);
    }

    [Fact]
    public void GeneratedMcpProject_HasCollections()
    {
        var generated = new GeneratedMcpProject();

        Assert.NotNull(generated.Tools);
        Assert.NotNull(generated.Resources);
        Assert.Empty(generated.Tools);
        Assert.Empty(generated.Resources);
    }
}
