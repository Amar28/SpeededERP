import { ThemeProvider } from '@mui/material/styles';
import { render, screen } from '@testing-library/react';
import { describe, expect, it } from 'vitest';

import { buildTheme } from '../theme/theme';
import { StatCard } from './StatCard';

function renderWithTheme(ui: React.ReactElement) {
  return render(<ThemeProvider theme={buildTheme('light')}>{ui}</ThemeProvider>);
}

describe('StatCard', () => {
  it('renders the label and value', () => {
    renderWithTheme(<StatCard label="Devis en attente" value={12} />);

    expect(screen.getByText('Devis en attente')).toBeInTheDocument();
    expect(screen.getByText('12')).toBeInTheDocument();
  });

  it('renders helper text when provided', () => {
    renderWithTheme(<StatCard label="Chiffre d'affaires" value="12 400 €" helperText="+8% vs mois dernier" />);

    expect(screen.getByText("+8% vs mois dernier")).toBeInTheDocument();
  });

  it('omits the sparkline when fewer than two trend points are supplied', () => {
    const { container } = renderWithTheme(
      <StatCard label="Factures" value={3} trend={[{ value: 1 }]} />,
    );

    expect(container.querySelector('.recharts-responsive-container')).toBeNull();
  });
});
