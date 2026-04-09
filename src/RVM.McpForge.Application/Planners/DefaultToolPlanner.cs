using RVM.McpForge.Domain.Entities;
using RVM.McpForge.Domain.Enums;
using RVM.McpForge.Domain.Models;

namespace RVM.McpForge.Application.Planners;

public class DefaultToolPlanner : IToolPlanner
{
    public GenerationPlan CreatePlan(AnalysisSnapshot snapshot, string serverName, string outputPath)
    {
        var plan = new GenerationPlan
        {
            McpServerName = serverName,
            OutputPath = outputPath
        };

        foreach (var table in snapshot.Tables)
        {
            plan.Tools.Add(new McpToolDefinition
            {
                Name = $"query_{table.TableName}",
                Description = $"Query records from {table.SchemaName}.{table.TableName}",
                Category = ToolCategory.Query,
                SourceTable = $"{table.SchemaName}.{table.TableName}",
                InputSchemaJson = BuildTableQuerySchema(table)
            });

            plan.Resources.Add(new McpResourceDefinition
            {
                Uri = $"db://{table.SchemaName}/{table.TableName}",
                Name = $"{table.SchemaName}.{table.TableName}",
                Description = $"Schema information for table {table.TableName}"
            });
        }

        foreach (var endpoint in snapshot.Endpoints)
        {
            var category = endpoint.HttpMethod switch
            {
                "GET" => ToolCategory.Query,
                "POST" or "PUT" or "PATCH" or "DELETE" => ToolCategory.Command,
                _ => ToolCategory.Query
            };

            plan.Tools.Add(new McpToolDefinition
            {
                Name = $"{endpoint.HttpMethod.ToLowerInvariant()}_{endpoint.ControllerName}_{endpoint.ActionName}",
                Description = $"{endpoint.HttpMethod} {endpoint.Route} - {endpoint.ActionName}",
                Category = category,
                SourceEndpoint = $"{endpoint.HttpMethod} {endpoint.Route}"
            });
        }

        plan.Tools.Add(new McpToolDefinition
        {
            Name = "get_schema",
            Description = "Get the full database/API schema overview",
            Category = ToolCategory.Schema,
            InputSchemaJson = "{\"type\":\"object\",\"properties\":{}}"
        });

        return plan;
    }

    private static string BuildTableQuerySchema(DiscoveredTable table)
    {
        var properties = new Dictionary<string, object>();
        foreach (var col in table.Columns)
        {
            properties[col.ColumnName] = new
            {
                type = MapDataType(col.DataType),
                description = $"Filter by {col.ColumnName} ({col.DataType})"
            };
        }

        var schema = new
        {
            type = "object",
            properties,
            required = Array.Empty<string>()
        };

        return System.Text.Json.JsonSerializer.Serialize(schema);
    }

    private static string MapDataType(string pgType) => pgType switch
    {
        "integer" or "bigint" or "smallint" => "number",
        "numeric" or "decimal" or "real" or "double precision" => "number",
        "boolean" => "boolean",
        _ => "string"
    };
}
