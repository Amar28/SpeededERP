import type { ApiClient } from './httpClient';

/** Mirrors `Domain.Aggregates.Tenants.TenantIsolationMode` — see Tenant.cs. */
export const TENANT_ISOLATION_MODES = ['Shared', 'Dedicated', 'OnPremiseSingleTenant'] as const;
export type TenantIsolationMode = (typeof TENANT_ISOLATION_MODES)[number];

export interface UserDto {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  status: string;
  roleIds: string[];
}

export interface RegisterUserRequest {
  email: string;
  firstName: string;
  lastName: string;
  initialPassword: string;
}

export interface ProvisionTenantRequest {
  companyName: string;
  slug: string;
  isolationMode: number; // wire value — see TENANT_ISOLATION_MODES index
}

export interface CreateRoleRequest {
  name: string;
  description?: string;
  permissions: string[];
}

/** Client for Core.Identity's REST surface — see UserEndpoints.cs / TenantEndpoints.cs / RoleEndpoints.cs. */
export class IdentityClient {
  constructor(private readonly http: ApiClient) {}

  getUserById(userId: string): Promise<UserDto> {
    return this.http.get<UserDto>(`/api/v1/identity/users/${userId}`);
  }

  registerUser(request: RegisterUserRequest): Promise<{ id: string }> {
    return this.http.post<{ id: string }>('/api/v1/identity/users', request);
  }

  assignRole(userId: string, roleId: string): Promise<void> {
    return this.http.post<void>(`/api/v1/identity/users/${userId}/roles/${roleId}`);
  }

  provisionTenant(request: ProvisionTenantRequest): Promise<{ id: string }> {
    return this.http.post<{ id: string }>('/api/v1/identity/tenants', request);
  }

  createRole(request: CreateRoleRequest): Promise<{ id: string }> {
    return this.http.post<{ id: string }>('/api/v1/identity/roles', request);
  }
}
