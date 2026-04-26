/**
 * RVM.McpForge — Gerador de Manual HTML
 *
 * Le os screenshots gerados pelo Playwright e produz um manual HTML standalone
 * com descritivos de cada funcionalidade.
 *
 * Uso:
 *   npx tsx docs/generate-html.ts
 *
 * Saida:
 *   docs/manual-usuario.html
 *   docs/manual-usuario.md
 */
import fs from 'fs';
import path from 'path';

const SCREENSHOTS_DIR = path.resolve(__dirname, 'screenshots');
const OUTPUT_HTML = path.resolve(__dirname, 'manual-usuario.html');
const OUTPUT_MD = path.resolve(__dirname, 'manual-usuario.md');

interface Section {
  id: string;
  title: string;
  description: string;
  screenshot: string;
  features: string[];
  tips?: string[];
}

const sections: Section[] = [
  {
    id: 'home',
    title: '1. Home / Introducao',
    description:
      'Pagina inicial do RVM.McpForge. Apresenta o gerador de MCP Servers com exemplos ' +
      'de uso e acesso rapido as funcionalidades principais.',
    screenshot: '01-home',
    features: [
      'Visao geral do que e um MCP Server',
      'Exemplos de casos de uso (Git, Database, API)',
      'Acesso rapido a criacao de projetos',
      'Documentacao inline das funcionalidades',
      'Status do sistema e ultimas geracoes',
    ],
    tips: [
      'O MCP Server gerado e compativel com Claude Desktop e outros clientes MCP.',
      'Comece criando um projeto para organizar seus servidores MCP.',
    ],
  },
  {
    id: 'projects',
    title: '2. Projetos',
    description:
      'Gerencie seus projetos de MCP Servers. Cada projeto agrupa servidores MCP ' +
      'relacionados, com controle de versao e configuracoes compartilhadas.',
    screenshot: '02-projects',
    features: [
      'Listagem de projetos com status e data de criacao',
      'Criar novo projeto com nome, descricao e tipo (Git, Database, REST API)',
      'Editar configuracoes do projeto',
      'Ver servidores MCP gerados por projeto',
      'Duplicar projeto como ponto de partida',
      'Arquivar ou excluir projetos',
    ],
    tips: [
      'Organize projetos por dominio (ex: "Financeiro", "CRM", "Infraestrutura").',
      'Projetos arquivados ficam visiveis em filtro separado mas nao aparecem na lista principal.',
    ],
  },
  {
    id: 'generated',
    title: '3. Servidores MCP Gerados',
    description:
      'Visualize e gerencie todos os MCP Servers gerados. Faca o download do codigo, ' +
      'copie as configuracoes para o Claude Desktop e veja o historico de versoes.',
    screenshot: '03-generated',
    features: [
      'Listagem de servidores MCP por projeto',
      'Preview do codigo gerado com syntax highlight',
      'Download do servidor MCP como arquivo TypeScript/Python',
      'Configuracao pronta para copiar no Claude Desktop (claude_desktop_config.json)',
      'Historico de versoes por servidor',
      'Regenerar servidor com novas configuracoes',
      'Status: ativo, em rascunho, deprecado',
    ],
    tips: [
      'Copie a configuracao do Claude Desktop e cole em ~/Library/Application Support/Claude/claude_desktop_config.json (Mac) ou %APPDATA%/Claude/claude_desktop_config.json (Windows).',
      'Apos adicionar a configuracao, reinicie o Claude Desktop para carregar o novo servidor.',
    ],
  },
];

// ---------------------------------------------------------------------------
// Gerar HTML
// ---------------------------------------------------------------------------
function imageToBase64(filePath: string): string | null {
  if (!fs.existsSync(filePath)) return null;
  const buffer = fs.readFileSync(filePath);
  return `data:image/png;base64,${buffer.toString('base64')}`;
}

function generateHTML(): string {
  const now = new Date().toLocaleDateString('pt-BR', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
  });

  let sectionsHtml = '';
  for (const s of sections) {
    const desktopPath = path.join(SCREENSHOTS_DIR, `${s.screenshot}--desktop.png`);
    const mobilePath = path.join(SCREENSHOTS_DIR, `${s.screenshot}--mobile.png`);
    const desktopImg = imageToBase64(desktopPath);
    const mobileImg = imageToBase64(mobilePath);

    const featuresHtml = s.features.map((f) => `<li>${f}</li>`).join('\n            ');
    const tipsHtml = s.tips
      ? `<div class="tips">
          <strong>Dicas:</strong>
          <ul>${s.tips.map((t) => `<li>${t}</li>`).join('\n            ')}</ul>
        </div>`
      : '';

    const screenshotsHtml = desktopImg
      ? `<div class="screenshots">
          <div class="screenshot-group">
            <span class="badge">Desktop</span>
            <img src="${desktopImg}" alt="${s.title} - Desktop" />
          </div>
          ${
            mobileImg
              ? `<div class="screenshot-group mobile">
              <span class="badge">Mobile</span>
              <img src="${mobileImg}" alt="${s.title} - Mobile" />
            </div>`
              : ''
          }
        </div>`
      : '<p class="no-screenshot"><em>Screenshot nao disponivel. Execute o script Playwright para gerar.</em></p>';

    sectionsHtml += `
    <section id="${s.id}">
      <h2>${s.title}</h2>
      <p class="description">${s.description}</p>
      <div class="features">
        <strong>Funcionalidades:</strong>
        <ul>
            ${featuresHtml}
        </ul>
      </div>
      ${tipsHtml}
      ${screenshotsHtml}
    </section>`;
  }

  return `<!DOCTYPE html>
<html lang="pt-BR">
<head>
  <meta charset="utf-8">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <title>RVM.McpForge - Manual do Usuario</title>
  <style>
    :root {
      --primary: #06b6d4;
      --surface: #ffffff;
      --bg: #f4f6fa;
      --text: #1e293b;
      --text-muted: #64748b;
      --border: #e2e8f0;
      --sidebar-bg: #083344;
      --accent: #06b6d4;
    }
    * { box-sizing: border-box; margin: 0; padding: 0; }
    body {
      font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
      background: var(--bg);
      color: var(--text);
      line-height: 1.6;
    }
    .container { max-width: 1100px; margin: 0 auto; padding: 2rem 1.5rem; }
    header {
      background: var(--sidebar-bg);
      color: white;
      padding: 3rem 1.5rem;
      text-align: center;
    }
    header h1 { font-size: 2rem; margin-bottom: 0.5rem; }
    header p { color: #67e8f9; font-size: 1rem; }
    header .version { color: #22d3ee; font-size: 0.85rem; margin-top: 0.5rem; }
    nav {
      background: var(--surface);
      border-bottom: 1px solid var(--border);
      padding: 1rem 1.5rem;
      position: sticky;
      top: 0;
      z-index: 100;
    }
    nav .container { padding: 0; }
    nav ul { list-style: none; display: flex; flex-wrap: wrap; gap: 0.5rem; }
    nav a {
      display: inline-block;
      padding: 0.35rem 0.75rem;
      border-radius: 0.5rem;
      font-size: 0.85rem;
      color: var(--text);
      text-decoration: none;
      background: var(--bg);
      transition: background 0.2s;
    }
    nav a:hover { background: var(--primary); color: white; }
    section {
      background: var(--surface);
      border: 1px solid var(--border);
      border-radius: 1rem;
      padding: 2rem;
      margin-bottom: 2rem;
    }
    section h2 {
      font-size: 1.5rem;
      color: var(--primary);
      margin-bottom: 1rem;
      padding-bottom: 0.5rem;
      border-bottom: 2px solid var(--border);
    }
    .description { font-size: 1.05rem; margin-bottom: 1.25rem; color: var(--text); }
    .features, .tips {
      background: var(--bg);
      border-radius: 0.75rem;
      padding: 1rem 1.25rem;
      margin-bottom: 1.25rem;
    }
    .features ul, .tips ul { margin-top: 0.5rem; padding-left: 1.25rem; }
    .features li, .tips li { margin-bottom: 0.35rem; }
    .tips { background: #ecfeff; border-left: 4px solid var(--accent); }
    .tips strong { color: var(--accent); }
    .screenshots {
      display: flex;
      gap: 1.5rem;
      margin-top: 1rem;
      align-items: flex-start;
    }
    .screenshot-group {
      position: relative;
      flex: 1;
      border: 1px solid var(--border);
      border-radius: 0.75rem;
      overflow: hidden;
    }
    .screenshot-group.mobile { flex: 0 0 200px; max-width: 200px; }
    .screenshot-group img { width: 100%; display: block; }
    .badge {
      position: absolute;
      top: 0.5rem;
      right: 0.5rem;
      background: var(--sidebar-bg);
      color: white;
      font-size: 0.7rem;
      padding: 0.2rem 0.5rem;
      border-radius: 0.35rem;
      font-weight: 600;
      text-transform: uppercase;
    }
    .no-screenshot {
      background: var(--bg);
      padding: 2rem;
      border-radius: 0.75rem;
      text-align: center;
      color: var(--text-muted);
    }
    footer {
      text-align: center;
      padding: 2rem 1rem;
      color: var(--text-muted);
      font-size: 0.85rem;
    }
    @media (max-width: 768px) {
      .screenshots { flex-direction: column; }
      .screenshot-group.mobile { max-width: 100%; flex: 1; }
      section { padding: 1.25rem; }
    }
    @media print {
      nav { display: none; }
      section { break-inside: avoid; page-break-inside: avoid; }
      .screenshots { flex-direction: column; }
      .screenshot-group.mobile { max-width: 250px; }
    }
  </style>
</head>
<body>
  <header>
    <h1>RVM.McpForge - Manual do Usuario</h1>
    <p>Gerador de MCP Servers — Guia Completo de Funcionalidades</p>
    <div class="version">Gerado em ${now} | RVM Tech</div>
  </header>

  <nav>
    <div class="container">
      <ul>
        ${sections.map((s) => `<li><a href="#${s.id}">${s.title}</a></li>`).join('\n        ')}
      </ul>
    </div>
  </nav>

  <div class="container">
    <section id="visao-geral">
      <h2>Visao Geral</h2>
      <p class="description">
        O <strong>RVM.McpForge</strong> gera MCP Servers (Model Context Protocol) automaticamente
        a partir de repositorios Git, schemas de banco de dados ou especificacoes de API REST.
        O servidor MCP gerado e compativel com Claude Desktop e outros clientes MCP.
      </p>
      <div class="features">
        <strong>Recursos principais:</strong>
        <ul>
          <li><strong>Geracao automatica</strong> — MCP Server a partir de Git, Database ou API</li>
          <li><strong>Multiplas linguagens</strong> — TypeScript ou Python</li>
          <li><strong>Configuracao pronta</strong> — claude_desktop_config.json gerado automaticamente</li>
          <li><strong>Projetos organizados</strong> — agrupe servidores MCP por dominio</li>
          <li><strong>Controle de versao</strong> — historico de geracoes por servidor</li>
          <li><strong>Preview inline</strong> — visualize o codigo antes de fazer download</li>
        </ul>
      </div>
    </section>

    ${sectionsHtml}
  </div>

  <footer>
    <p>RVM Tech &mdash; Gerador de MCP Servers</p>
    <p>Documento gerado automaticamente com Playwright + TypeScript</p>
  </footer>
</body>
</html>`;
}

// ---------------------------------------------------------------------------
// Gerar Markdown
// ---------------------------------------------------------------------------
function generateMarkdown(): string {
  const now = new Date().toLocaleDateString('pt-BR', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
  });

  let md = `# RVM.McpForge - Manual do Usuario

> Gerador de MCP Servers — Guia Completo de Funcionalidades
>
> Gerado em ${now} | RVM Tech

---

## Visao Geral

O **RVM.McpForge** gera MCP Servers (Model Context Protocol) automaticamente
a partir de repositorios Git, schemas de banco de dados ou especificacoes de API REST.
O servidor MCP gerado e compativel com Claude Desktop e outros clientes MCP.

**Recursos principais:**
- **Geracao automatica** — MCP Server a partir de Git, Database ou API
- **Multiplas linguagens** — TypeScript ou Python
- **Configuracao pronta** — claude_desktop_config.json gerado automaticamente
- **Projetos organizados** — agrupe servidores MCP por dominio
- **Controle de versao** — historico de geracoes por servidor
- **Preview inline** — visualize o codigo antes de fazer download

---

`;

  for (const s of sections) {
    const desktopExists = fs.existsSync(
      path.join(SCREENSHOTS_DIR, `${s.screenshot}--desktop.png`),
    );

    md += `## ${s.title}\n\n`;
    md += `${s.description}\n\n`;
    md += `**Funcionalidades:**\n`;
    for (const f of s.features) {
      md += `- ${f}\n`;
    }
    md += '\n';

    if (s.tips) {
      md += `> **Dicas:**\n`;
      for (const t of s.tips) {
        md += `> - ${t}\n`;
      }
      md += '\n';
    }

    if (desktopExists) {
      md += `| Desktop | Mobile |\n`;
      md += `|---------|--------|\n`;
      md += `| ![${s.title} - Desktop](screenshots/${s.screenshot}--desktop.png) | ![${s.title} - Mobile](screenshots/${s.screenshot}--mobile.png) |\n`;
    } else {
      md += `*Screenshot nao disponivel. Execute o script Playwright para gerar.*\n`;
    }
    md += '\n---\n\n';
  }

  md += `## Informacoes Tecnicas

| Item | Detalhe |
|------|---------|
| **Backend** | ASP.NET Core + Blazor Server |
| **Banco de dados** | PostgreSQL 16 + EF Core |
| **Geracao de codigo** | Roslyn + T4 Templates |
| **Protocolo** | MCP (Model Context Protocol) — SDK oficial |
| **Deploy** | Docker Compose + Nginx |

---

*Documento gerado automaticamente com Playwright + TypeScript — RVM Tech*
`;

  return md;
}

// ---------------------------------------------------------------------------
// Main
// ---------------------------------------------------------------------------
const html = generateHTML();
fs.writeFileSync(OUTPUT_HTML, html, 'utf-8');
console.log(`HTML gerado: ${OUTPUT_HTML}`);

const md = generateMarkdown();
fs.writeFileSync(OUTPUT_MD, md, 'utf-8');
console.log(`Markdown gerado: ${OUTPUT_MD}`);
