# RVM.McpForge

Automated MCP Server (Model Context Protocol) generator from Git repositories and PostgreSQL database schemas. Analyzes source code with Roslyn or introspects the database via `information_schema`, plans the tools/resources, and generates a ready-to-use .NET MCP Server project.

**Portfolio project** — showcases code generation, static analysis (Roslyn), database introspection, and the MCP protocol.

## Features

- MCP Server generation from Git repositories (Roslyn analysis of controllers, entities, and services)
- MCP Server generation from PostgreSQL schema (tables, columns, foreign keys)
- Blazor Server dashboard with project CRUD and listing of generated MCP Servers
- Pipeline: `Pending → Analyzing → Analyzed → Generating → Ready`
- Scriban templates (`.sbn`) for `.csproj`, `Program.cs`, and tool classes
- API Key authentication (`X-Api-Key`)
- Rate limiting (120 req/min per IP), Correlation ID, health check
- 38 xUnit tests

## Stack

| Component | Technology |
|---|---|
| Runtime | .NET 10 |
| API + UI | ASP.NET Core + Blazor Server |
| Database | PostgreSQL 16 (EF Core 10) |
| Code analysis | Roslyn (Microsoft.CodeAnalysis 4.13) |
| DB introspection | Npgsql + information_schema |
| Code generation | Scriban 5.12 |
| Git cloning | LibGit2Sharp 0.30 |
| Logging | Serilog + Seq |
| Container | Docker (multi-stage) |

## Getting Started

**Requirements:** Docker, .NET 10 SDK

```bash
# Start with Docker Compose (PostgreSQL + application)
docker compose up -d

# Access
# Dashboard: http://localhost:5120
# Dev API Key: mcpforge-dev-key-2026
```

**Run without Docker:**

```bash
cd src/RVM.McpForge.API
dotnet run
```

The default connection string points to `localhost:5432`. Adjust `appsettings.json` or use environment variables.

## Structure

```
RVM.McpForge/
├── src/
│   ├── RVM.McpForge.Domain/          # Entities, enums, interfaces, models
│   ├── RVM.McpForge.Infrastructure/  # EF Core, repositories, migrations
│   ├── RVM.McpForge.Application/     # Analyzers, Planners, Generators, Services
│   └── RVM.McpForge.API/             # Controllers, DTOs, Blazor, Auth
├── tests/
│   └── RVM.McpForge.Tests/           # 38 xUnit tests
├── docs/
│   └── MANUAL.md                     # Full usage and API reference
├── docker-compose.yml
├── Dockerfile
└── global.json
```

## Key Endpoints

Base: `/api/forge` — all require `X-Api-Key`

| Method | Route | Description |
|---|---|---|
| `GET` | `/api/forge/projects` | List projects |
| `POST` | `/api/forge/projects` | Create project |
| `POST` | `/api/forge/projects/{id}/analyze` | Run analysis |
| `POST` | `/api/forge/projects/{id}/generate` | Generate MCP Server |
| `GET` | `/api/forge/generated/{projectId}` | List generated servers |

Full documentation in [`docs/MANUAL.md`](docs/MANUAL.md).

## License

MIT
