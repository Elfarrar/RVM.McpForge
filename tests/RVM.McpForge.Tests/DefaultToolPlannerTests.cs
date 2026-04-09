using RVM.McpForge.Application.Planners;
using RVM.McpForge.Domain.Entities;
using RVM.McpForge.Domain.Enums;

namespace RVM.McpForge.Tests;

public class DefaultToolPlannerTests
{
    private readonly DefaultToolPlanner _planner = new();

    [Fact]
    public void CreatePlan_WithTables_GeneratesQueryTools()
    {
        var snapshot = new AnalysisSnapshot
        {
            SourceType = SourceType.Database,
            Tables =
            [
                new DiscoveredTable
                {
                    TableName = "users",
                    SchemaName = "public",
                    Columns =
                    [
                        new DiscoveredColumn { ColumnName = "id", DataType = "integer", IsPrimaryKey = true },
                        new DiscoveredColumn { ColumnName = "name", DataType = "text" }
                    ]
                }
            ]
        };

        var plan = _planner.CreatePlan(snapshot, "TestServer", "/tmp/out");

        Assert.Equal("TestServer", plan.McpServerName);
        Assert.Contains(plan.Tools, t => t.Name == "query_users" && t.Category == ToolCategory.Query);
        Assert.Contains(plan.Resources, r => r.Uri == "db://public/users");
        Assert.Contains(plan.Tools, t => t.Name == "get_schema" && t.Category == ToolCategory.Schema);
    }

    [Fact]
    public void CreatePlan_WithEndpoints_GeneratesApiTools()
    {
        var snapshot = new AnalysisSnapshot
        {
            SourceType = SourceType.Git,
            Endpoints =
            [
                new DiscoveredEndpoint
                {
                    HttpMethod = "GET",
                    Route = "api/users",
                    ControllerName = "UsersController",
                    ActionName = "GetAll"
                },
                new DiscoveredEndpoint
                {
                    HttpMethod = "POST",
                    Route = "api/users",
                    ControllerName = "UsersController",
                    ActionName = "Create"
                }
            ]
        };

        var plan = _planner.CreatePlan(snapshot, "ApiServer", "/tmp/out");

        Assert.Contains(plan.Tools, t => t.Name == "get_UsersController_GetAll" && t.Category == ToolCategory.Query);
        Assert.Contains(plan.Tools, t => t.Name == "post_UsersController_Create" && t.Category == ToolCategory.Command);
    }

    [Fact]
    public void CreatePlan_Empty_GeneratesSchemaToolOnly()
    {
        var snapshot = new AnalysisSnapshot { SourceType = SourceType.Git };

        var plan = _planner.CreatePlan(snapshot, "EmptyServer", "/tmp/out");

        Assert.Single(plan.Tools);
        Assert.Equal("get_schema", plan.Tools[0].Name);
        Assert.Empty(plan.Resources);
    }
}
