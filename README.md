# RVM.McpForge

Gerador automatizado de MCP Servers (Model Context Protocol) a partir de repositorios Git e schemas de banco de dados PostgreSQL. Analisa o codigo-fonte com Roslyn ou introspecciona o banco via `information_schema`, planeja as tools/resources e gera um projeto .NET pronto para uso como MCP Server.

**Projeto de portfolio** — demonstra geracao de codigo, analise estatica (Roslyn), introspeccao de banco e o protocolo MCP.

## Funcionalidades

- Geracao de MCP Server a partir de repositorio Git (analise Roslyn de controllers, entidades e servicos)
- Geracao de MCP Server a partir de schema PostgreSQL (tabelas, colunas, foreign keys)
- Dashboard Blazor Server com CRUD de projetos e listagem de MCP Servers gerados
- Pipeline: `Pending → Analyzing → Analyzed → Generating → Ready`
- Templates Scriban (`.sbn`) para `.csproj`, `Program.cs` e classes de tools
- Autenticacao por API Key (`X-Api-Key`)
- Rate limiting (120 req/min por IP), Correlation ID, health check
- 38 testes xUnit

## Stack

| Componente | Tecnologia |
|---|---|
| Runtime | .NET 10 |
| API + UI | ASP.NET Core + Blazor Server |
| Banco de dados | PostgreSQL 16 (EF Core 10) |
| Analise de codigo | Roslyn (Microsoft.CodeAnalysis 4.13) |
| Analise de banco | Npgsql + information_schema |
| Geracao de codigo | Scriban 5.12 |
| Clonagem Git | LibGit2Sharp 0.30 |
| Logs | Serilog + Seq |
| Container | Docker (multi-stage) |

## Como Rodar

**Pre-requisitos:** Docker, .NET 10 SDK

```bash
# Subir via Docker Compose (PostgreSQL + aplicacao)
docker compose up -d

# Acesso
# Dashboard: http://localhost:5120
# API Key dev: mcpforge-dev-key-2026
```

**Rodar sem Docker:**

```bash
cd src/RVM.McpForge.API
dotnet run
```

A connection string padrao aponta para `localhost:5432`. Ajuste `appsettings.json` ou use variaveis de ambiente.

## Estrutura

```
RVM.McpForge/
├── src/
│   ├── RVM.McpForge.Domain/          # Entidades, enums, interfaces, models
│   ├── RVM.McpForge.Infrastructure/  # EF Core, repositorios, migrations
│   ├── RVM.McpForge.Application/     # Analyzers, Planners, Generators, Services
│   └── RVM.McpForge.API/             # Controllers, DTOs, Blazor, Auth
├── tests/
│   └── RVM.McpForge.Tests/           # 38 testes xUnit
├── docs/
│   └── MANUAL.md                     # Manual completo de uso e API
├── docker-compose.yml
├── Dockerfile
└── global.json
```

## Endpoints Principais

Base: `/api/forge` — todos exigem `X-Api-Key`

| Metodo | Rota | Descricao |
|---|---|---|
| `GET` | `/api/forge/projects` | Lista projetos |
| `POST` | `/api/forge/projects` | Cria projeto |
| `POST` | `/api/forge/projects/{id}/analyze` | Executa analise |
| `POST` | `/api/forge/projects/{id}/generate` | Gera MCP Server |
| `GET` | `/api/forge/generated/{projectId}` | Lista gerados |

Documentacao completa em [`docs/MANUAL.md`](docs/MANUAL.md).

## Licenca

MIT
