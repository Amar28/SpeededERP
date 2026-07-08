import { SpeededErpApiClient } from '@speedederp/api-client';

const baseUrl = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5080';

export const apiClient = new SpeededErpApiClient({ baseUrl });
