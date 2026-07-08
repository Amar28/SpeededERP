import { zodResolver } from '@hookform/resolvers/zod';
import Alert from '@mui/material/Alert';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Paper from '@mui/material/Paper';
import Stack from '@mui/material/Stack';
import TextField from '@mui/material/TextField';
import Typography from '@mui/material/Typography';
import { useState } from 'react';
import { useForm } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { useNavigate } from 'react-router-dom';
import { z } from 'zod';

import { useAuth } from '../auth/AuthContext';

const loginSchema = z.object({
  email: z.email(),
  password: z.string().min(1),
});

type LoginFormValues = z.infer<typeof loginSchema>;

export function LoginPage() {
  const { t } = useTranslation();
  const { login } = useAuth();
  const navigate = useNavigate();
  const [error, setError] = useState<string | null>(null);

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<LoginFormValues>({ resolver: zodResolver(loginSchema) });

  const onSubmit = async (values: LoginFormValues) => {
    setError(null);
    try {
      await login(values.email, values.password);
      navigate('/', { replace: true });
    } catch {
      setError(t('login.error'));
    }
  };

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
      <Paper variant="outlined" sx={{ p: 4, width: '100%', maxWidth: 420 }}>
        <Typography variant="h5" component="h1" gutterBottom>
          {t('login.title')}
        </Typography>
        <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
          {t('login.subtitle')}
        </Typography>

        <Box component="form" onSubmit={handleSubmit(onSubmit)} noValidate>
          <Stack spacing={2}>
            {error ? <Alert severity="error">{error}</Alert> : null}
            <TextField
              label={t('login.email')}
              type="email"
              autoComplete="email"
              error={!!errors.email}
              helperText={errors.email ? t('common.requiredField') : undefined}
              {...register('email')}
            />
            <TextField
              label={t('login.password')}
              type="password"
              autoComplete="current-password"
              error={!!errors.password}
              helperText={errors.password ? t('common.requiredField') : undefined}
              {...register('password')}
            />
            <Button type="submit" variant="contained" size="large" disabled={isSubmitting}>
              {isSubmitting ? t('login.loading') : t('login.submit')}
            </Button>
          </Stack>
        </Box>
      </Paper>
    </Box>
  );
}
