# TESTING — RVM.McpForge

## Testes Unitarios (xUnit)

**Cobertura:** 38 testes cobrindo analyzers, planners, generators e controllers.

```bash
# Rodar todos os testes
dotnet test

# Com verbosidade detalhada
dotnet test --logger "console;verbosity=detailed"

# Filtrar por categoria
dotnet test --filter "Category=Analyzer"
dotnet test --filter "Category=Generator"
```

**Projeto de testes:** `tests/RVM.McpForge.Tests/`

### O que e testado

| Area | Exemplos |
|---|---|
| `RoslynAnalyzer` | Descoberta de controllers, entidades, servicos em codigo C# |
| `DatabaseAnalyzer` | Parsing de tabelas, colunas, FKs |
| `DefaultToolPlanner` | Mapeamento de endpoints para tool categories |
| `ScribanMcpGenerator` | Renderizacao de templates `.sbn` |
| `ForgeController` | Endpoints REST (create, analyze, generate, list) |

## Testes E2E (Playwright)

Os testes E2E do McpForge ficam no repositorio centralizado **RVM.E2E** (`tests/mcpforge.spec.ts`).

```bash
# No repo RVM.E2E
npx playwright test mcpforge.spec.ts

# Com UI interativa
npx playwright test mcpforge.spec.ts --ui
```

Os testes E2E cobrem o dashboard Blazor (`/`, `/projects`, `/generated`) e o fluxo de criacao de projeto via API.

## Ambiente de Testes

Os testes unitarios rodam completamente in-process, sem dependencias externas (PostgreSQL, Git, rede). Mocks e builders estao em `tests/RVM.McpForge.Tests/Helpers/`.

Para testes de integracao que exijam banco real, suba o Docker Compose antes:

```bash
docker compose up -d postgres
dotnet test --filter "Category=Integration"
```
