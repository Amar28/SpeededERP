import { ApiError, type ApiClientConfig } from './httpClient';

/** Standard OAuth2 token response (RFC 6749) — see AuthEndpoints.cs / OpenIddictServerRegistration.cs. */
export interface TokenResponse {
  access_token: string;
  token_type: string;
  expires_in: number;
  refresh_token?: string;
  scope?: string;
}

/**
 * `/connect/token` is OpenIddict's OAuth2 endpoint: it expects a form-urlencoded
 * body, not JSON, per RFC 6749 — deliberately not routed through `ApiClient.post`.
 * The password grant is for first-party tooling only (see AuthEndpoints.cs); the
 * Admin/Tenant portals should migrate to Authorization Code + PKCE via a real login
 * redirect once that interactive flow has a hosted consent/login page.
 */
export async function login(
  config: ApiClientConfig,
  username: string,
  password: string,
): Promise<TokenResponse> {
  const body = new URLSearchParams({
    grant_type: 'password',
    username,
    password,
    scope: 'openid profile email offline_access speeded_erp_api',
  });

  const url = config.baseUrl.endsWith('/') ? `${config.baseUrl}connect/token` : `${config.baseUrl}/connect/token`;

  const response = await fetch(url, {
    method: 'POST',
    headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
    body,
  });

  const text = await response.text();
  const data = text.length > 0 ? JSON.parse(text) : undefined;

  if (!response.ok) {
    throw new ApiError(response.status, data, 'Échec de la connexion.');
  }

  return data as TokenResponse;
}

export async function refreshToken(config: ApiClientConfig, refresh_token: string): Promise<TokenResponse> {
  const body = new URLSearchParams({ grant_type: 'refresh_token', refresh_token });
  const url = config.baseUrl.endsWith('/') ? `${config.baseUrl}connect/token` : `${config.baseUrl}/connect/token`;

  const response = await fetch(url, {
    method: 'POST',
    headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
    body,
  });

  const text = await response.text();
  const data = text.length > 0 ? JSON.parse(text) : undefined;

  if (!response.ok) {
    throw new ApiError(response.status, data, 'Échec du rafraîchissement du jeton.');
  }

  return data as TokenResponse;
}

/**
 * In-memory-only token store, deliberately not localStorage (XSS exfiltration risk
 * for a long-lived access token) — a page reload requires logging in again until the
 * Authorization Code + PKCE flow with an httpOnly refresh cookie is wired.
 */
export class TokenStore {
  private token: string | null = null;

  set(token: string | null): void {
    this.token = token;
  }

  get(): string | null {
    return this.token;
  }
}
