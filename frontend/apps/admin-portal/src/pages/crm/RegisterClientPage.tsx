import { zodResolver } from '@hookform/resolvers/zod';
import Alert from '@mui/material/Alert';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Paper from '@mui/material/Paper';
import Stack from '@mui/material/Stack';
import TextField from '@mui/material/TextField';
import Typography from '@mui/material/Typography';
import { useMutation } from '@tanstack/react-query';
import { useForm } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { z } from 'zod';

import { apiClient } from '../../lib/apiClient';

const registerClientSchema = z.object({
  name: z.string().min(1),
  street: z.string().min(1),
  city: z.string().min(1),
  postalCode: z.string().min(1),
  countryIso2: z.string().length(2).toUpperCase(),
  phone: z.string().optional(),
});

type RegisterClientFormValues = z.infer<typeof registerClientSchema>;

/** Reference implementation of architecture section 10's exact scenario — see RegisterClientCommand. */
export function RegisterClientPage() {
  const { t } = useTranslation();

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<RegisterClientFormValues>({ resolver: zodResolver(registerClientSchema) });

  const mutation = useMutation({
    mutationFn: (values: RegisterClientFormValues) => apiClient.crm.registerClient(values),
    onSuccess: () => reset(),
  });

  return (
    <Paper variant="outlined" sx={{ p: 4, maxWidth: 560 }}>
      <Typography variant="h5" component="h1" gutterBottom>
        {t('crm.registerClient.title')}
      </Typography>

      <Box component="form" onSubmit={handleSubmit((values) => mutation.mutate(values))} noValidate>
        <Stack spacing={2}>
          {mutation.isSuccess ? (
            <Alert severity="success">
              {t('crm.registerClient.success', { id: mutation.data.id })}
            </Alert>
          ) : null}
          {mutation.isError ? <Alert severity="error">{mutation.error.message}</Alert> : null}

          <TextField
            label={t('crm.registerClient.name')}
            error={!!errors.name}
            helperText={errors.name ? t('common.requiredField') : undefined}
            {...register('name')}
          />
          <TextField
            label={t('crm.registerClient.street')}
            error={!!errors.street}
            helperText={errors.street ? t('common.requiredField') : undefined}
            {...register('street')}
          />
          <Stack direction="row" spacing={2}>
            <TextField
              label={t('crm.registerClient.city')}
              fullWidth
              error={!!errors.city}
              helperText={errors.city ? t('common.requiredField') : undefined}
              {...register('city')}
            />
            <TextField
              label={t('crm.registerClient.postalCode')}
              fullWidth
              error={!!errors.postalCode}
              helperText={errors.postalCode ? t('common.requiredField') : undefined}
              {...register('postalCode')}
            />
          </Stack>
          <TextField
            label={t('crm.registerClient.countryIso2')}
            error={!!errors.countryIso2}
            helperText={errors.countryIso2 ? t('common.requiredField') : undefined}
            {...register('countryIso2')}
          />
          <TextField label={t('crm.registerClient.phone')} {...register('phone')} />

          <Button type="submit" variant="contained" size="large" disabled={mutation.isPending}>
            {mutation.isPending ? t('common.loading') : t('crm.registerClient.submit')}
          </Button>
        </Stack>
      </Box>
    </Paper>
  );
}
