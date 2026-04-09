using Npgsql;
using RVM.McpForge.Domain.Entities;
using RVM.McpForge.Domain.Enums;

namespace RVM.McpForge.Application.Analyzers;

public class DatabaseAnalyzer : ISourceAnalyzer
{
    public async Task<AnalysisSnapshot> AnalyzeAsync(ForgeProject project, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(project.ConnectionString))
            throw new InvalidOperationException("ConnectionString is required for Database analysis.");

        var snapshot = new AnalysisSnapshot
        {
            ForgeProjectId = project.Id,
            SourceType = SourceType.Database
        };

        await using var conn = new NpgsqlConnection(project.ConnectionString);
        await conn.OpenAsync(ct);

        await DiscoverTablesAsync(conn, snapshot, ct);
        await DiscoverRelationshipsAsync(conn, snapshot, ct);

        snapshot.TotalTables = snapshot.Tables.Count;
        snapshot.TotalColumns = snapshot.Tables.Sum(t => t.Columns.Count);
        snapshot.TotalRelationships = snapshot.Relationships.Count;

        return snapshot;
    }

    private static async Task DiscoverTablesAsync(NpgsqlConnection conn, AnalysisSnapshot snapshot, CancellationToken ct)
    {
        const string tablesSql = """
            SELECT table_schema, table_name
            FROM information_schema.tables
            WHERE table_schema NOT IN ('pg_catalog', 'information_schema')
              AND table_type = 'BASE TABLE'
            ORDER BY table_schema, table_name
            """;

        await using var cmd = new NpgsqlCommand(tablesSql, conn);
        await using var reader = await cmd.ExecuteReaderAsync(ct);

        var tables = new List<DiscoveredTable>();
        while (await reader.ReadAsync(ct))
        {
            tables.Add(new DiscoveredTable
            {
                AnalysisSnapshotId = snapshot.Id,
                SchemaName = reader.GetString(0),
                TableName = reader.GetString(1)
            });
        }
        await reader.CloseAsync();

        foreach (var table in tables)
        {
            await DiscoverColumnsAsync(conn, table, ct);
            snapshot.Tables.Add(table);
        }
    }

    private static async Task DiscoverColumnsAsync(NpgsqlConnection conn, DiscoveredTable table, CancellationToken ct)
    {
        const string columnsSql = """
            SELECT c.column_name, c.data_type, c.is_nullable, c.column_default, c.character_maximum_length,
                   CASE WHEN tc.constraint_type = 'PRIMARY KEY' THEN true ELSE false END as is_pk
            FROM information_schema.columns c
            LEFT JOIN information_schema.key_column_usage kcu
              ON c.table_schema = kcu.table_schema AND c.table_name = kcu.table_name AND c.column_name = kcu.column_name
            LEFT JOIN information_schema.table_constraints tc
              ON kcu.constraint_name = tc.constraint_name AND tc.constraint_type = 'PRIMARY KEY'
            WHERE c.table_schema = @schema AND c.table_name = @table
            ORDER BY c.ordinal_position
            """;

        await using var cmd = new NpgsqlCommand(columnsSql, conn);
        cmd.Parameters.AddWithValue("schema", table.SchemaName);
        cmd.Parameters.AddWithValue("table", table.TableName);
        await using var reader = await cmd.ExecuteReaderAsync(ct);

        while (await reader.ReadAsync(ct))
        {
            table.Columns.Add(new DiscoveredColumn
            {
                DiscoveredTableId = table.Id,
                ColumnName = reader.GetString(0),
                DataType = reader.GetString(1),
                IsNullable = reader.GetString(2) == "YES",
                DefaultValue = reader.IsDBNull(3) ? null : reader.GetString(3),
                MaxLength = reader.IsDBNull(4) ? null : reader.GetInt32(4),
                IsPrimaryKey = reader.GetBoolean(5)
            });
        }
    }

    private static async Task DiscoverRelationshipsAsync(NpgsqlConnection conn, AnalysisSnapshot snapshot, CancellationToken ct)
    {
        const string fkSql = """
            SELECT tc.constraint_name, kcu.table_name, kcu.column_name,
                   ccu.table_name AS ref_table, ccu.column_name AS ref_column
            FROM information_schema.table_constraints tc
            JOIN information_schema.key_column_usage kcu
              ON tc.constraint_name = kcu.constraint_name
            JOIN information_schema.constraint_column_usage ccu
              ON tc.constraint_name = ccu.constraint_name
            WHERE tc.constraint_type = 'FOREIGN KEY'
              AND tc.table_schema NOT IN ('pg_catalog', 'information_schema')
            """;

        await using var cmd = new NpgsqlCommand(fkSql, conn);
        await using var reader = await cmd.ExecuteReaderAsync(ct);

        while (await reader.ReadAsync(ct))
        {
            snapshot.Relationships.Add(new DiscoveredRelationship
            {
                AnalysisSnapshotId = snapshot.Id,
                ConstraintName = reader.GetString(0),
                SourceTable = reader.GetString(1),
                SourceColumn = reader.GetString(2),
                TargetTable = reader.GetString(3),
                TargetColumn = reader.GetString(4)
            });
        }
    }
}
