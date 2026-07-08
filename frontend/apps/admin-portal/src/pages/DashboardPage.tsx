import Grid from '@mui/material/Grid2';
import Typography from '@mui/material/Typography';
import { StatCard } from '@speedederp/ui-kit';
import { useTranslation } from 'react-i18next';

/**
 * Placeholder KPI layout (architecture section 14) — real widgets will bind to
 * ReportDefinition-backed queries once Core.Metadata's dashboard endpoints exist.
 */
export function DashboardPage() {
  const { t } = useTranslation();

  return (
    <>
      <Typography variant="h4" component="h1" gutterBottom>
        {t('dashboard.title')}
      </Typography>
      <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
        {t('dashboard.placeholder')}
      </Typography>
      <Grid container spacing={2}>
        <Grid size={{ xs: 12, sm: 4 }}>
          <StatCard label={t('dashboard.kpi.clients')} value="—" />
        </Grid>
        <Grid size={{ xs: 12, sm: 4 }}>
          <StatCard label={t('dashboard.kpi.devisEnCours')} value="—" />
        </Grid>
        <Grid size={{ xs: 12, sm: 4 }}>
          <StatCard label={t('dashboard.kpi.chiffreAffaires')} value="—" />
        </Grid>
      </Grid>
    </>
  );
}
