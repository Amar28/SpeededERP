import { SpeededErpApiClient } from '@speedederp/api-client';

/** Points at the Gateway in production, direct-to-Api in local dev (architecture section 04/30). */
const baseUrl = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5080';

export const apiClient = new SpeededErpApiClient({ baseUrl });
