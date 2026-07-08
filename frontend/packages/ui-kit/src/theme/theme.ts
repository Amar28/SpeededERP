import { createTheme, type PaletteMode, type Theme } from '@mui/material/styles';

/**
 * Professional, neutral/blue ERP palette — deliberately restrained (this is
 * dense application UI, not a marketing/editorial surface).
 */
const brand = {
  main: '#2455A4',
  light: '#5C82C6',
  dark: '#123467',
};

const neutralLight = {
  background: {
    default: '#F5F6F8',
    paper: '#FFFFFF',
  },
  text: {
    primary: '#1B2430',
    secondary: '#5B6472',
  },
  divider: '#E1E4E9',
};

const neutralDark = {
  background: {
    default: '#12161C',
    paper: '#1A1F27',
  },
  text: {
    primary: '#E8EAED',
    secondary: '#A2AAB6',
  },
  divider: '#2A2F38',
};

export function buildTheme(mode: PaletteMode): Theme {
  const isLight = mode === 'light';

  return createTheme({
    palette: {
      mode,
      primary: brand,
      secondary: {
        main: '#3E7C6A',
      },
      error: { main: '#C4453C' },
      warning: { main: '#B8791A' },
      success: { main: '#2E7D53' },
      info: { main: '#2C7FB0' },
      background: isLight ? neutralLight.background : neutralDark.background,
      text: isLight ? neutralLight.text : neutralDark.text,
      divider: isLight ? neutralLight.divider : neutralDark.divider,
    },
    shape: {
      borderRadius: 8,
    },
    typography: {
      fontFamily: [
        'Inter',
        '-apple-system',
        'BlinkMacSystemFont',
        '"Segoe UI"',
        'Roboto',
        'Helvetica',
        'Arial',
        'sans-serif',
      ].join(','),
      h1: { fontWeight: 600 },
      h2: { fontWeight: 600 },
      h3: { fontWeight: 600 },
      h4: { fontWeight: 600 },
      h5: { fontWeight: 600 },
      h6: { fontWeight: 600 },
      button: { textTransform: 'none', fontWeight: 600 },
    },
    components: {
      MuiButton: {
        styleOverrides: {
          root: { borderRadius: 6 },
        },
      },
      MuiPaper: {
        styleOverrides: {
          root: { backgroundImage: 'none' },
        },
      },
      MuiAppBar: {
        styleOverrides: {
          root: {
            backgroundColor: isLight ? '#FFFFFF' : neutralDark.background.paper,
            color: isLight ? neutralLight.text.primary : neutralDark.text.primary,
          },
        },
      },
    },
  });
}
