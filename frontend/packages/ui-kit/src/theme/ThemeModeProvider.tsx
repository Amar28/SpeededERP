import CssBaseline from '@mui/material/CssBaseline';
import { ThemeProvider, type PaletteMode } from '@mui/material/styles';
import {
  createContext,
  useContext,
  useMemo,
  useState,
  type ReactNode,
} from 'react';

import { buildTheme } from './theme';

interface ThemeModeContextValue {
  mode: PaletteMode;
  toggleMode: () => void;
  setMode: (mode: PaletteMode) => void;
}

const ThemeModeContext = createContext<ThemeModeContextValue | null>(null);

const STORAGE_KEY = 'speedederp:theme-mode';

function readInitialMode(): PaletteMode {
  if (typeof window === 'undefined') return 'light';
  const stored = window.localStorage.getItem(STORAGE_KEY);
  if (stored === 'light' || stored === 'dark') return stored;
  return window.matchMedia?.('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
}

export interface ThemeModeProviderProps {
  children: ReactNode;
  defaultMode?: PaletteMode;
}

/** App-level provider wiring the SpeededERP MUI theme with a persisted light/dark toggle. */
export function ThemeModeProvider({ children, defaultMode }: ThemeModeProviderProps) {
  const [mode, setModeState] = useState<PaletteMode>(() => defaultMode ?? readInitialMode());

  const setMode = (next: PaletteMode) => {
    setModeState(next);
    if (typeof window !== 'undefined') {
      window.localStorage.setItem(STORAGE_KEY, next);
    }
  };

  const value = useMemo<ThemeModeContextValue>(
    () => ({
      mode,
      toggleMode: () => setMode(mode === 'light' ? 'dark' : 'light'),
      setMode,
    }),
    [mode],
  );

  const theme = useMemo(() => buildTheme(mode), [mode]);

  return (
    <ThemeModeContext.Provider value={value}>
      <ThemeProvider theme={theme}>
        <CssBaseline />
        {children}
      </ThemeProvider>
    </ThemeModeContext.Provider>
  );
}

export function useThemeMode(): ThemeModeContextValue {
  const ctx = useContext(ThemeModeContext);
  if (!ctx) throw new Error('useThemeMode must be used within a ThemeModeProvider');
  return ctx;
}
