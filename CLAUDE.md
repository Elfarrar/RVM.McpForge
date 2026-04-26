# RVM.McpForge

## Visao Geral

RVM.McpForge e um gerador automatizado de MCP Servers (Model Context Protocol) a partir de repositorios Git e schemas PostgreSQL. O sistema usa Roslyn para analisar codigo C# (controllers, entidades, servicos) ou introspecciona o banco via `information_schema`, planeja quais tools/resources MCP criar e gera um projeto .NET completo via templates Scriban. Projeto de portfolio — sem dados reais de clientes.

O ciclo de vida de um projeto Forge: `Pending → Analyzing → Analyzed → Generating → Ready` (ou `Failed` em qualquer etapa).

## Stack

- **Runtime:** .NET 10
- **API + UI:** ASP.NET Core Controllers + Blazor Server (Interactive SSR)
- **Banco:** PostgreSQL 16 via EF Core 10 + Npgsql
- **Analise de codigo:** Microsoft.CodeAnalysis (Roslyn) 4.13
- **Geracao de codigo:** Scriban 5.12 (templates `.sbn`)
- **Git clone:** LibGit2Sharp 0.30
- **Auth:** API Key via header `X-Api-Key`
- **Logs:** Serilog (Console JSON + Seq opcional)
- **Container:** Docker multi-stage

## Estrutura

```
src/
  RVM.McpForge.Domain/          # Entidades, Enums, Interfaces, Models
  RVM.McpForge.Infrastructure/  # EF Core DbContext, Repositories, Migrations
  RVM.McpForge.Application/     # Analyzers, Planners, Generators, Services
  RVM.McpForge.API/             # Controllers, DTOs, Auth middleware, Blazor, Health
tests/
  RVM.McpForge.Tests/           # 38 testes xUnit
docs/
  MANUAL.md                     # Referencia completa de uso e API
```

## Componentes Chave

| Componente | Responsabilidade |
|---|---|
| `ForgeOrchestrator` | Orquestra todo o fluxo (clone → analise → geracao) |
| `GitCloneService` | Clona repos Git via LibGit2Sharp |
| `RoslynAnalyzer` | Analisa `.cs` files — descobre controllers, entidades, servicos |
| `DatabaseAnalyzer` | Introspecciona PostgreSQL via `information_schema` |
| `DefaultToolPlanner` | Converte `AnalysisSnapshot` em `GenerationPlan` |
| `ScribanMcpGenerator` | Renderiza templates `.sbn` e gera projeto .NET MCP |
| `ForgeController` | Endpoints REST do pipeline (criar/analisar/gerar/listar) |

## Convencoes

- **Auth:** todos os endpoints da API exigem `X-Api-Key`. Chave dev: `mcpforge-dev-key-2026`.
- **Rate limiting:** 120 req/min por IP (Fixed Window). Rotas `/_blazor`, `/health`, `/css`, `/js` sao isentas.
- **Correlation ID:** toda request recebe `X-Correlation-Id` automatico (gerado se nao enviado).
- **Path base:** configuravel via `App__PathBase` quando atras de reverse proxy.
- **Templates Scriban:** ficam em `Application/Generators/Templates/` com extensao `.sbn`.
- **Migrations EF Core:** rodar `dotnet ef migrations add <nome>` na camada Infrastructure.

## Como Rodar

```bash
# Docker (recomendado)
docker compose up -d
# Dashboard: http://localhost:5120

# Direto (requer PostgreSQL local)
cd src/RVM.McpForge.API
dotnet run
```

Variaveis de ambiente principais:

| Variavel | Padrao dev |
|---|---|
| `ConnectionStrings__DefaultConnection` | `Host=localhost;Database=rvmmcpforge;Username=postgres;Password=postgres` |
| `ApiKeys__Keys__0__Key` | `mcpforge-dev-key-2026` |
| `Seq__ServerUrl` | _(vazio — desabilitado)_ |
| `App__PathBase` | _(vazio)_ |

## Testes

```bash
dotnet test
```

38 testes xUnit cobrindo analyzers, planners, generators e controllers. Ver `TESTING.md` para detalhes.

## Decisoes de Arquitetura

- **Clean Architecture** com 4 camadas: Domain, Infrastructure, Application, API. Sem dependencia circular.
- **Roslyn como biblioteca** (sem `dotnet build` externo): analise in-process via `CSharpSyntaxTree.ParseText`.
- **Scriban escolhido** sobre T4/Razor para templates por ser portavel, testavel e sem dependencia de MSBuild.
- **LibGit2Sharp** para clone Git sem fork de processo git CLI.
- **MCP gerado usa stdio transport** (ModelContextProtocol 0.2.0-preview.1) — padrao para integracao com Claude Desktop.
