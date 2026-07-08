import { zodResolver } from '@hookform/resolvers/zod';
import Alert from '@mui/material/Alert';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import MenuItem from '@mui/material/MenuItem';
import Paper from '@mui/material/Paper';
import Stack from '@mui/material/Stack';
import TextField from '@mui/material/TextField';
import Typography from '@mui/material/Typography';
import { TENANT_ISOLATION_MODES, type TenantIsolationMode } from '@speedederp/api-client';
import { useMutation } from '@tanstack/react-query';
import { Controller, useForm } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { z } from 'zod';

import { apiClient } from '../lib/apiClient';

const provisionSchema = z.object({
  companyName: z.string().min(1),
  slug: z
    .string()
    .min(3)
    .max(63)
    .regex(/^[a-z0-9-]+$/, 'Minuscules, chiffres et tirets uniquement'),
  isolationMode: z.enum(TENANT_ISOLATION_MODES),
});

type ProvisionFormValues = z.infer<typeof provisionSchema>;

const isolationModeLabelKeys: Record<TenantIsolationMode, string> = {
  Shared: 'tenant.provision.isolationModeShared',
  Dedicated: 'tenant.provision.isolationModeDedicated',
  OnPremiseSingleTenant: 'tenant.provision.isolationModeOnPremise',
};

export function ProvisionTenantPage() {
  const { t } = useTranslation();

  const {
    control,
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<ProvisionFormValues>({
    resolver: zodResolver(provisionSchema),
    defaultValues: { isolationMode: 'Shared' },
  });

  const mutation = useMutation({
    mutationFn: (values: ProvisionFormValues) =>
      apiClient.identity.provisionTenant({
        companyName: values.companyName,
        slug: values.slug,
        isolationMode: TENANT_ISOLATION_MODES.indexOf(values.isolationMode),
      }),
    onSuccess: () => reset(),
  });

  return (
    <Box
      sx={{
        minHeight: '100vh',
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center',
        px: 2,
      }}
    >
      <Paper variant="outlined" sx={{ p: 4, width: '100%', maxWidth: 480 }}>
        <Typography variant="h5" component="h1" gutterBottom>
          {t('tenant.provision.title')}
        </Typography>
        <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
          {t('tenant.provision.subtitle')}
        </Typography>

        <Box component="form" onSubmit={handleSubmit((values) => mutation.mutate(values))} noValidate>
          <Stack spacing={2}>
            {mutation.isSuccess ? (
              <Alert severity="success">
                {t('tenant.provision.success', { id: mutation.data.id })}
              </Alert>
            ) : null}
            {mutation.isError ? <Alert severity="error">{mutation.error.message}</Alert> : null}

            <TextField
              label={t('tenant.provision.companyName')}
              error={!!errors.companyName}
              helperText={errors.companyName ? t('common.requiredField') : undefined}
              {...register('companyName')}
            />
            <TextField
              label={t('tenant.provision.slug')}
              error={!!errors.slug}
              helperText={errors.slug?.message ?? t('common.requiredField')}
              {...register('slug')}
            />
            <Controller
              name="isolationMode"
              control={control}
              render={({ field }) => (
                <TextField select label={t('tenant.provision.isolationMode')} {...field}>
                  {TENANT_ISOLATION_MODES.map((mode) => (
                    <MenuItem key={mode} value={mode}>
                      {t(isolationModeLabelKeys[mode])}
                    </MenuItem>
                  ))}
                </TextField>
              )}
            />

            <Button type="submit" variant="contained" size="large" disabled={mutation.isPending}>
              {mutation.isPending ? t('common.loading') : t('tenant.provision.submit')}
            </Button>
          </Stack>
        </Box>
      </Paper>
    </Box>
  );
}
