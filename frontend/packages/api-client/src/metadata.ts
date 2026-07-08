import type { ApiClient } from './httpClient';

export interface FieldDefinitionDto {
  id: string;
  key: string;
  labelKey: string;
  type: number; // wire value — see @speedederp/form-engine's FIELD_TYPES for the string union
  isRequired: boolean;
  order: number;
  listOptions: string[];
}

export interface AddFieldDefinitionRequest {
  key: string;
  labelKey: string;
  type: number;
  isRequired: boolean;
  order: number;
  listOptions?: string[];
  validationRuleJson?: string;
}

export interface FormDefinitionDto {
  id: string;
  entityDefinitionId: string;
  name: string;
  layoutJson: string;
  version: number;
  isPublished: boolean;
}

export interface SaveFormDefinitionRequest {
  formDefinitionId?: string;
  entityDefinitionId: string;
  name: string;
  layoutJson: string;
}

/** Client for the no-code engine's REST surface — see FieldDefinitionEndpoints.cs / FormDefinitionEndpoints.cs. */
export class MetadataClient {
  constructor(private readonly http: ApiClient) {}

  getFieldDefinitions(entityDefinitionId: string): Promise<FieldDefinitionDto[]> {
    return this.http.get<FieldDefinitionDto[]>(`/api/v1/metadata/entities/${entityDefinitionId}/fields`);
  }

  addFieldDefinition(entityDefinitionId: string, request: AddFieldDefinitionRequest): Promise<{ id: string }> {
    return this.http.post<{ id: string }>(`/api/v1/metadata/entities/${entityDefinitionId}/fields`, request);
  }

  getFormDefinition(formDefinitionId: string): Promise<FormDefinitionDto> {
    return this.http.get<FormDefinitionDto>(`/api/v1/metadata/forms/${formDefinitionId}`);
  }

  saveFormDefinition(request: SaveFormDefinitionRequest): Promise<{ id: string }> {
    return this.http.post<{ id: string }>('/api/v1/metadata/forms', request);
  }

  publishFormDefinition(formDefinitionId: string): Promise<void> {
    return this.http.post<void>(`/api/v1/metadata/forms/${formDefinitionId}/publish`);
  }
}
