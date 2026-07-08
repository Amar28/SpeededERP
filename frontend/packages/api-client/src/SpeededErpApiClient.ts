import { login, refreshToken, TokenStore, type TokenResponse } from './auth';
import { CrmClient } from './crm';
import { ApiClient, type ApiClientConfig } from './httpClient';
import { IdentityClient } from './identity';
import { MetadataClient } from './metadata';

export interface SpeededErpApiClientOptions {
  baseUrl: string;
}

/** Single entry point apps import — composes the per-module clients over one shared ApiClient/token store. */
export class SpeededErpApiClient {
  readonly tokenStore = new TokenStore();
  readonly identity: IdentityClient;
  readonly metadata: MetadataClient;
  readonly crm: CrmClient;

  private readonly config: ApiClientConfig;

  constructor(options: SpeededErpApiClientOptions) {
    this.config = {
      baseUrl: options.baseUrl,
      getAccessToken: () => this.tokenStore.get(),
    };

    const http = new ApiClient(this.config);
    this.identity = new IdentityClient(http);
    this.metadata = new MetadataClient(http);
    this.crm = new CrmClient(http);
  }

  async login(username: string, password: string): Promise<TokenResponse> {
    const token = await login(this.config, username, password);
    this.tokenStore.set(token.access_token);
    return token;
  }

  async refresh(refresh_token: string): Promise<TokenResponse> {
    const token = await refreshToken(this.config, refresh_token);
    this.tokenStore.set(token.access_token);
    return token;
  }

  logout(): void {
    this.tokenStore.set(null);
  }
}
