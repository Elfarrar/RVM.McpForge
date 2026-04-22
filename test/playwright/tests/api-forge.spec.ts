import { expect, test } from '@playwright/test';

const defaultBaseUrl = process.env.MCPFORGE_BASE_URL ?? 'https://mcpforge.lab.rvmtech.com.br';

test.describe('Forge API', () => {
  test.skip(
    process.env.MCPFORGE_RUN_SMOKE !== '1',
    'Defina MCPFORGE_RUN_SMOKE=1 para rodar o smoke contra um ambiente real.',
  );

  test('GET /api/forge/projects — requer autenticacao (401)', async ({ request, baseURL }) => {
    const currentBaseUrl = baseURL ?? defaultBaseUrl;
    const response = await request.get(`${currentBaseUrl}/api/forge/projects`);
    expect([401, 403]).toContain(response.status());
  });

  test('GET /api/forge/projects/{id} — id invalido retorna 401 ou 404', async ({ request, baseURL }) => {
    const currentBaseUrl = baseURL ?? defaultBaseUrl;
    const fakeId = '00000000-0000-0000-0000-000000000000';
    const response = await request.get(`${currentBaseUrl}/api/forge/projects/${fakeId}`);
    expect([401, 403, 404]).toContain(response.status());
  });

  test('POST /api/forge/projects — sem auth retorna 401', async ({ request, baseURL }) => {
    const currentBaseUrl = baseURL ?? defaultBaseUrl;
    const response = await request.post(`${currentBaseUrl}/api/forge/projects`, { data: {} });
    expect([400, 401, 403]).toContain(response.status());
  });

  test('POST /api/forge/projects/{id}/analyze — sem auth retorna 401', async ({ request, baseURL }) => {
    const currentBaseUrl = baseURL ?? defaultBaseUrl;
    const fakeId = '00000000-0000-0000-0000-000000000000';
    const response = await request.post(`${currentBaseUrl}/api/forge/projects/${fakeId}/analyze`);
    expect([401, 403, 404]).toContain(response.status());
  });

  test('GET /api/forge/generated/{projectId} — sem auth retorna 401', async ({ request, baseURL }) => {
    const currentBaseUrl = baseURL ?? defaultBaseUrl;
    const fakeId = '00000000-0000-0000-0000-000000000000';
    const response = await request.get(`${currentBaseUrl}/api/forge/generated/${fakeId}`);
    expect([401, 403, 404]).toContain(response.status());
  });

  test('DELETE /api/forge/projects/{id} — sem auth retorna 401', async ({ request, baseURL }) => {
    const currentBaseUrl = baseURL ?? defaultBaseUrl;
    const fakeId = '00000000-0000-0000-0000-000000000000';
    const response = await request.delete(`${currentBaseUrl}/api/forge/projects/${fakeId}`);
    expect([401, 403, 404]).toContain(response.status());
  });
});
