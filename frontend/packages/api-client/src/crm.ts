import type { ApiClient } from './httpClient';

export interface LigneDevisDto {
  id: string;
  description: string;
  quantite: number;
  prixUnitaire: number;
  montantLigne: number;
}

export interface DevisDto {
  id: string;
  clientId: string;
  statut: string;
  devise: string;
  montant: number;
  lignes: LigneDevisDto[];
}

export interface CreateDevisRequest {
  clientId: string;
  devise?: string;
}

export interface AddLigneDevisRequest {
  description: string;
  quantite: number;
  prixUnitaire: number;
}

export interface RegisterClientRequest {
  name: string;
  street: string;
  city: string;
  postalCode: string;
  countryIso2: string;
  phone?: string;
  customFields?: Record<string, unknown>;
}

/** Client for CRM's REST surface — see DevisEndpoints.cs / ClientEndpoints.cs (architecture sections 07/09/10). */
export class CrmClient {
  constructor(private readonly http: ApiClient) {}

  getDevisById(devisId: string): Promise<DevisDto> {
    return this.http.get<DevisDto>(`/api/v1/crm/devis/${devisId}`);
  }

  createDevis(request: CreateDevisRequest): Promise<{ id: string }> {
    return this.http.post<{ id: string }>('/api/v1/crm/devis', request);
  }

  addLigneDevis(devisId: string, request: AddLigneDevisRequest): Promise<void> {
    return this.http.post<void>(`/api/v1/crm/devis/${devisId}/lignes`, request);
  }

  sendDevis(devisId: string): Promise<void> {
    return this.http.post<void>(`/api/v1/crm/devis/${devisId}/envoyer`);
  }

  acceptDevis(devisId: string): Promise<void> {
    return this.http.post<void>(`/api/v1/crm/devis/${devisId}/accepter`);
  }

  registerClient(request: RegisterClientRequest): Promise<{ id: string }> {
    return this.http.post<{ id: string }>('/api/v1/crm/clients', request);
  }
}
