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
import { useState } from 'react';
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

const adminSchema = z.object({
  firstName: z.string().min(1),
  lastName: z.string().min(1),
  email: z.string().email(),
  password: z.string().min(12, 'Minimum 12 caractères'),
});

type AdminFormValues = z.infer<typeof adminSchema>;

const isolationModeLabelKeys: Record<TenantIsolationMode, string> = {
  Shared: 'tenant.provision.isolationModeShared',
  Dedicated: 'tenant.provision.isolationModeDedicated',
  OnPremiseSingleTenant: 'tenant.provision.isolationModeOnPremise',
};

const pageBoxSx = { minHeight: '100vh', display: 'flex', alignItems: 'center', justifyContent: 'center', px: 2 } as const;

/**
 * Two steps: provision the organization (Tenant), then register its first user —
 * who becomes the tenant's administrator, since there is no other way yet to obtain
 * an authenticated caller to create/assign a role through the normal, auth-gated
 * endpoints (see RegisterUserCommandHandler). The second step's request carries
 * X-Tenant-Slug so TenantResolutionMiddleware can resolve the brand-new tenant for
 * this still-anonymous call.
 */
export function ProvisionTenantPage() {
  const { t } = useTranslation();
  const [provisionedTenant, setProvisionedTenant] = useState<{ slug: string; companyName: string } | null>(null);

  const {
    control,
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<ProvisionFormValues>({
    resolver: zodResolver(provisionSchema),
    defaultValues: { isolationMode: 'Shared' },
  });

  const provisionMutation = useMutation({
    mutationFn: (values: ProvisionFormValues) =>
      apiClient.identity.provisionTenant({
        companyName: values.companyName,
        slug: values.slug,
        isolationMode: TENANT_ISOLATION_MODES.indexOf(values.isolationMode),
      }),
  });

  const {
    register: registerAdmin,
    handleSubmit: handleSubmitAdmin,
    formState: { errors: adminErrors },
  } = useForm<AdminFormValues>({
    resolver: zodResolver(adminSchema),
  });

  const adminMutation = useMutation({
    mutationFn: (values: AdminFormValues) =>
      apiClient.identity.registerUser(
        {
          email: values.email,
          firstName: values.firstName,
          lastName: values.lastName,
          initialPassword: values.password,
        },
        provisionedTenant?.slug,
      ),
  });

  if (adminMutation.isSuccess) {
    return (
      <Box sx={pageBoxSx}>
        <Paper variant="outlined" sx={{ p: 4, width: '100%', maxWidth: 480 }}>
          <Alert severity="success">{t('tenant.registerAdmin.success')}</Alert>
        </Paper>
      </Box>
    );
  }

  if (provisionedTenant) {
    return (
      <Box sx={pageBoxSx}>
        <Paper variant="outlined" sx={{ p: 4, width: '100%', maxWidth: 480 }}>
          <Typography variant="h5" component="h1" gutterBottom>
            {t('tenant.registerAdmin.title')}
          </Typography>
          <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
            {t('tenant.registerAdmin.subtitle', { companyName: provisionedTenant.companyName })}
          </Typography>

          <Box component="form" onSubmit={handleSubmitAdmin((values) => adminMutation.mutate(values))} noValidate>
            <Stack spacing={2}>
              {adminMutation.isError ? <Alert severity="error">{adminMutation.error.message}</Alert> : null}

              <TextField
                label={t('tenant.registerAdmin.firstName')}
                error={!!adminErrors.firstName}
                helperText={adminErrors.firstName ? t('common.requiredField') : undefined}
                {...registerAdmin('firstName')}
              />
              <TextField
                label={t('tenant.registerAdmin.lastName')}
                error={!!adminErrors.lastName}
                helperText={adminErrors.lastName ? t('common.requiredField') : undefined}
                {...registerAdmin('lastName')}
              />
              <TextField
                label={t('tenant.registerAdmin.email')}
                type="email"
                error={!!adminErrors.email}
                helperText={adminErrors.email ? t('common.requiredField') : undefined}
                {...registerAdmin('email')}
              />
              <TextField
                label={t('tenant.registerAdmin.password')}
                type="password"
                error={!!adminErrors.password}
                helperText={adminErrors.password?.message}
                {...registerAdmin('password')}
              />

              <Button type="submit" variant="contained" size="large" disabled={adminMutation.isPending}>
                {adminMutation.isPending ? t('common.loading') : t('tenant.registerAdmin.submit')}
              </Button>
            </Stack>
          </Box>
        </Paper>
      </Box>
    );
  }

  return (
    <Box sx={pageBoxSx}>
      <Paper variant="outlined" sx={{ p: 4, width: '100%', maxWidth: 480 }}>
        <Typography variant="h5" component="h1" gutterBottom>
          {t('tenant.provision.title')}
        </Typography>
        <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
          {t('tenant.provision.subtitle')}
        </Typography>

        <Box
          component="form"
          onSubmit={handleSubmit((values) =>
            provisionMutation.mutate(values, {
              onSuccess: () => setProvisionedTenant({ slug: values.slug, companyName: values.companyName }),
            }),
          )}
          noValidate
        >
          <Stack spacing={2}>
            {provisionMutation.isError ? <Alert severity="error">{provisionMutation.error.message}</Alert> : null}

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

            <Button type="submit" variant="contained" size="large" disabled={provisionMutation.isPending}>
              {provisionMutation.isPending ? t('common.loading') : t('tenant.provision.submit')}
            </Button>
          </Stack>
        </Box>
      </Paper>
    </Box>
  );
}
