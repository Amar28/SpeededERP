import Alert from '@mui/material/Alert';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Chip from '@mui/material/Chip';
import Divider from '@mui/material/Divider';
import Paper from '@mui/material/Paper';
import Stack from '@mui/material/Stack';
import Table from '@mui/material/Table';
import TableBody from '@mui/material/TableBody';
import TableCell from '@mui/material/TableCell';
import TableHead from '@mui/material/TableHead';
import TableRow from '@mui/material/TableRow';
import TextField from '@mui/material/TextField';
import Typography from '@mui/material/Typography';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { useState } from 'react';
import { useTranslation } from 'react-i18next';

import { apiClient } from '../../lib/apiClient';

/**
 * Exercises the full reference flow from architecture sections 07/09: create a
 * Devis, add lignes, send it, then accept it — the exact sequence that, server-side,
 * ends with an IntegrationEvent enqueued through the Outbox (AcceptDevisCommandHandler).
 */
export function DevisPage() {
  const { t } = useTranslation();
  const queryClient = useQueryClient();

  const [devisId, setDevisId] = useState('');
  const [loadedId, setLoadedId] = useState<string | null>(null);
  const [clientId, setClientId] = useState('');
  const [ligne, setLigne] = useState({ description: '', quantite: '1', prixUnitaire: '0' });

  const devisQuery = useQuery({
    queryKey: ['devis', loadedId],
    queryFn: () => apiClient.crm.getDevisById(loadedId!),
    enabled: loadedId !== null,
  });

  const invalidate = () => queryClient.invalidateQueries({ queryKey: ['devis', loadedId] });

  const createMutation = useMutation({
    mutationFn: () => apiClient.crm.createDevis({ clientId, devise: 'EUR' }),
    onSuccess: (result) => setLoadedId(result.id),
  });

  const addLigneMutation = useMutation({
    mutationFn: () =>
      apiClient.crm.addLigneDevis(loadedId!, {
        description: ligne.description,
        quantite: Number(ligne.quantite),
        prixUnitaire: Number(ligne.prixUnitaire),
      }),
    onSuccess: () => {
      setLigne({ description: '', quantite: '1', prixUnitaire: '0' });
      invalidate();
    },
  });

  const sendMutation = useMutation({
    mutationFn: () => apiClient.crm.sendDevis(loadedId!),
    onSuccess: invalidate,
  });

  const acceptMutation = useMutation({
    mutationFn: () => apiClient.crm.acceptDevis(loadedId!),
    onSuccess: invalidate,
  });

  const devis = devisQuery.data;

  return (
    <Stack spacing={3} sx={{ maxWidth: 720 }}>
      <Paper variant="outlined" sx={{ p: 4 }}>
        <Typography variant="h5" component="h1" gutterBottom>
          {t('crm.devis.title')}
        </Typography>

        <Stack direction="row" spacing={2} alignItems="flex-end" sx={{ mb: 2 }}>
          <TextField
            label={t('crm.devis.devisId')}
            value={devisId}
            onChange={(e) => setDevisId(e.target.value)}
            fullWidth
          />
          <Button variant="outlined" onClick={() => setLoadedId(devisId)}>
            {t('crm.devis.load')}
          </Button>
        </Stack>

        <Divider sx={{ my: 2 }} />

        <Typography variant="subtitle1" gutterBottom>
          {t('crm.devis.create')}
        </Typography>
        <Stack direction="row" spacing={2}>
          <TextField
            label={t('crm.devis.clientId')}
            value={clientId}
            onChange={(e) => setClientId(e.target.value)}
            fullWidth
          />
          <Button
            variant="contained"
            disabled={!clientId || createMutation.isPending}
            onClick={() => createMutation.mutate()}
          >
            {t('crm.devis.create')}
          </Button>
        </Stack>
      </Paper>

      {devisQuery.isError ? <Alert severity="error">{(devisQuery.error as Error).message}</Alert> : null}

      {devis ? (
        <Paper variant="outlined" sx={{ p: 4 }}>
          <Stack direction="row" spacing={2} alignItems="center" sx={{ mb: 2 }}>
            <Chip label={devis.statut} color="primary" />
            <Typography variant="body2" color="text.secondary">
              {t('crm.devis.devise')}: {devis.devise}
            </Typography>
            <Typography variant="body2" color="text.secondary">
              {t('crm.devis.montant')}: {devis.montant}
            </Typography>
          </Stack>

          <Typography variant="subtitle1" gutterBottom>
            {t('crm.devis.lignes')}
          </Typography>
          <Table size="small">
            <TableHead>
              <TableRow>
                <TableCell>{t('crm.devis.description')}</TableCell>
                <TableCell align="right">{t('crm.devis.quantite')}</TableCell>
                <TableCell align="right">{t('crm.devis.prixUnitaire')}</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {devis.lignes.map((l) => (
                <TableRow key={l.id}>
                  <TableCell>{l.description}</TableCell>
                  <TableCell align="right">{l.quantite}</TableCell>
                  <TableCell align="right">{l.prixUnitaire}</TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>

          {devis.statut === 'Brouillon' ? (
            <Stack direction="row" spacing={2} sx={{ mt: 2 }}>
              <TextField
                label={t('crm.devis.description')}
                value={ligne.description}
                onChange={(e) => setLigne((s) => ({ ...s, description: e.target.value }))}
              />
              <TextField
                label={t('crm.devis.quantite')}
                type="number"
                value={ligne.quantite}
                onChange={(e) => setLigne((s) => ({ ...s, quantite: e.target.value }))}
                sx={{ width: 120 }}
              />
              <TextField
                label={t('crm.devis.prixUnitaire')}
                type="number"
                value={ligne.prixUnitaire}
                onChange={(e) => setLigne((s) => ({ ...s, prixUnitaire: e.target.value }))}
                sx={{ width: 140 }}
              />
              <Button
                variant="outlined"
                disabled={!ligne.description || addLigneMutation.isPending}
                onClick={() => addLigneMutation.mutate()}
              >
                {t('crm.devis.addLigne')}
              </Button>
            </Stack>
          ) : null}

          <Box sx={{ mt: 3 }}>
            <Stack direction="row" spacing={2}>
              {devis.statut === 'Brouillon' ? (
                <Button variant="contained" onClick={() => sendMutation.mutate()} disabled={sendMutation.isPending}>
                  {t('crm.devis.send')}
                </Button>
              ) : null}
              {devis.statut === 'Envoye' ? (
                <Button
                  variant="contained"
                  color="success"
                  onClick={() => acceptMutation.mutate()}
                  disabled={acceptMutation.isPending}
                >
                  {t('crm.devis.accept')}
                </Button>
              ) : null}
            </Stack>
          </Box>
        </Paper>
      ) : null}
    </Stack>
  );
}
