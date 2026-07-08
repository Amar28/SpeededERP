/**
 * RFC 7807 problem shape produced by `BuildingBlocks.Web.Errors.ErrorMapping.ToProblem`
 * — the single translation point every backend Minimal API endpoint uses (architecture
 * section 19), so every failed request in this client surfaces the same shape.
 */
export interface ProblemDetails {
  type?: string;
  title: string;
  status: number;
  detail?: string;
  errorType?: string;
  [key: string]: unknown;
}

export class ApiError extends Error {
  readonly status: number;
  readonly problem: ProblemDetails | undefined;

  constructor(status: number, problem: ProblemDetails | undefined, fallbackMessage: string) {
    super(problem?.detail ?? problem?.title ?? fallbackMessage);
    this.name = 'ApiError';
    this.status = status;
    this.problem = problem;
  }
}

export interface ApiClientConfig {
  /** Base URL of the backend (through the Gateway in production — architecture section 04/30). */
  baseUrl: string;
  /** Resolves the current access token, if any — set by the app after a successful login. */
  getAccessToken?: () => string | null;
}

export interface RequestOptions {
  query?: Record<string, string | number | boolean | undefined>;
  signal?: AbortSignal;
}

function buildUrl(baseUrl: string, path: string, query?: RequestOptions['query']): string {
  const url = new URL(path.replace(/^\//, ''), baseUrl.endsWith('/') ? baseUrl : `${baseUrl}/`);
  if (query) {
    for (const [key, value] of Object.entries(query)) {
      if (value !== undefined) {
        url.searchParams.set(key, String(value));
      }
    }
  }
  return url.toString();
}

/**
 * Thin, dependency-free fetch wrapper shared by every module-specific client
 * (identity.ts, metadata.ts, crm.ts) — attaches the Bearer token, serializes JSON,
 * and normalizes failures into `ApiError`/`ProblemDetails`.
 */
export class ApiClient {
  constructor(private readonly config: ApiClientConfig) {}

  buildHeaders(hasBody: boolean): Headers {
    const headers = new Headers();
    if (hasBody) {
      headers.set('Content-Type', 'application/json');
    }
    headers.set('Accept', 'application/json');

    const token = this.config.getAccessToken?.();
    if (token) {
      headers.set('Authorization', `Bearer ${token}`);
    }

    return headers;
  }

  buildUrl(path: string, options?: RequestOptions): string {
    return buildUrl(this.config.baseUrl, path, options?.query);
  }

  async request<TResponse>(
    method: 'GET' | 'POST' | 'PUT' | 'DELETE',
    path: string,
    body?: unknown,
    options?: RequestOptions,
  ): Promise<TResponse> {
    const response = await fetch(this.buildUrl(path, options), {
      method,
      headers: this.buildHeaders(body !== undefined),
      body: body !== undefined ? JSON.stringify(body) : undefined,
      signal: options?.signal,
    });

    if (response.status === 204) {
      return undefined as TResponse;
    }

    const text = await response.text();
    const data: unknown = text.length > 0 ? JSON.parse(text) : undefined;

    if (!response.ok) {
      throw new ApiError(response.status, data as ProblemDetails | undefined, response.statusText);
    }

    return data as TResponse;
  }

  get<TResponse>(path: string, options?: RequestOptions): Promise<TResponse> {
    return this.request<TResponse>('GET', path, undefined, options);
  }

  post<TResponse>(path: string, body?: unknown, options?: RequestOptions): Promise<TResponse> {
    return this.request<TResponse>('POST', path, body, options);
  }

  put<TResponse>(path: string, body?: unknown, options?: RequestOptions): Promise<TResponse> {
    return this.request<TResponse>('PUT', path, body, options);
  }

  delete<TResponse>(path: string, options?: RequestOptions): Promise<TResponse> {
    return this.request<TResponse>('DELETE', path, undefined, options);
  }
}
