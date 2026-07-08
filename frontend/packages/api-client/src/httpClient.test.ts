import { afterEach, describe, expect, it, vi } from 'vitest';

import { ApiClient } from './httpClient';

describe('ApiClient', () => {
  describe('buildUrl', () => {
    it('joins the base URL and a relative path', () => {
      const client = new ApiClient({ baseUrl: 'https://api.example.com' });

      expect(client.buildUrl('/api/v1/crm/devis/123')).toBe(
        'https://api.example.com/api/v1/crm/devis/123',
      );
    });

    it('appends query parameters, skipping undefined values', () => {
      const client = new ApiClient({ baseUrl: 'https://api.example.com' });

      const url = client.buildUrl('/api/v1/crm/devis', {
        query: { entityType: 'CRM.Client', entityId: undefined, page: 2 },
      });

      expect(url).toBe('https://api.example.com/api/v1/crm/devis?entityType=CRM.Client&page=2');
    });
  });

  describe('buildHeaders', () => {
    it('sets Content-Type only when the request has a body', () => {
      const client = new ApiClient({ baseUrl: 'https://api.example.com' });

      expect(client.buildHeaders(false).has('Content-Type')).toBe(false);
      expect(client.buildHeaders(true).get('Content-Type')).toBe('application/json');
    });

    it('attaches the Bearer token when getAccessToken returns one', () => {
      const client = new ApiClient({
        baseUrl: 'https://api.example.com',
        getAccessToken: () => 'the-token',
      });

      expect(client.buildHeaders(false).get('Authorization')).toBe('Bearer the-token');
    });

    it('omits Authorization when there is no token', () => {
      const client = new ApiClient({ baseUrl: 'https://api.example.com', getAccessToken: () => null });

      expect(client.buildHeaders(false).has('Authorization')).toBe(false);
    });
  });

  describe('request', () => {
    const originalFetch = globalThis.fetch;

    afterEach(() => {
      globalThis.fetch = originalFetch;
    });

    it('parses a successful JSON response', async () => {
      globalThis.fetch = vi.fn().mockResolvedValue(
        new Response(JSON.stringify({ id: 'abc' }), { status: 200 }),
      );
      const client = new ApiClient({ baseUrl: 'https://api.example.com' });

      const result = await client.get<{ id: string }>('/api/v1/crm/devis/abc');

      expect(result).toEqual({ id: 'abc' });
    });

    it('returns undefined for a 204 No Content response', async () => {
      globalThis.fetch = vi.fn().mockResolvedValue(new Response(null, { status: 204 }));
      const client = new ApiClient({ baseUrl: 'https://api.example.com' });

      const result = await client.post<void>('/api/v1/crm/devis/abc/envoyer');

      expect(result).toBeUndefined();
    });

    it('throws ApiError with the parsed ProblemDetails on failure', async () => {
      globalThis.fetch = vi.fn().mockResolvedValue(
        new Response(
          JSON.stringify({ title: 'Devis.NotFound', status: 404, detail: 'Devis introuvable.' }),
          { status: 404 },
        ),
      );
      const client = new ApiClient({ baseUrl: 'https://api.example.com' });

      await expect(client.get('/api/v1/crm/devis/missing')).rejects.toMatchObject({
        status: 404,
        message: 'Devis introuvable.',
      });
    });
  });
});
