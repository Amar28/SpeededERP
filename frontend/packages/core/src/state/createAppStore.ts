import { create, type StateCreator } from 'zustand';
import { devtools } from 'zustand/middleware';

/**
 * Thin wrapper around zustand's `create` that wires up the redux-devtools
 * middleware and gives every SpeededERP store a consistent creation pattern.
 *
 * Usage:
 * ```ts
 * interface AuthState {
 *   accessToken: string | null;
 *   setAccessToken: (token: string | null) => void;
 * }
 *
 * export const useAuthStore = createAppStore<AuthState>('auth', (set) => ({
 *   accessToken: null,
 *   setAccessToken: (accessToken) => set({ accessToken }),
 * }));
 * ```
 */
export function createAppStore<T extends object>(
  name: string,
  initializer: StateCreator<T, [['zustand/devtools', never]], [], T>,
) {
  return create<T>()(devtools(initializer, { name }));
}
