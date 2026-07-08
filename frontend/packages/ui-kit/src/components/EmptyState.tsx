import InboxOutlinedIcon from '@mui/icons-material/InboxOutlined';
import Box from '@mui/material/Box';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import type { ReactNode } from 'react';

export interface EmptyStateProps {
  title: string;
  description?: string;
  icon?: ReactNode;
  action?: ReactNode;
}

/** Placeholder shown for lists/tables with no data yet. */
export function EmptyState({ title, description, icon, action }: EmptyStateProps) {
  return (
    <Stack
      alignItems="center"
      justifyContent="center"
      spacing={1.5}
      sx={{
        py: 6,
        px: 3,
        textAlign: 'center',
        color: 'text.secondary',
      }}
    >
      <Box sx={{ fontSize: 40, color: 'text.disabled', display: 'flex' }}>
        {icon ?? <InboxOutlinedIcon fontSize="inherit" />}
      </Box>
      <Typography variant="subtitle1" color="text.primary" sx={{ fontWeight: 600 }}>
        {title}
      </Typography>
      {description ? <Typography variant="body2">{description}</Typography> : null}
      {action}
    </Stack>
  );
}
