import TextField from '@mui/material/TextField';

import type { WidgetProps } from '../registry';

export function TextBoxWidget({ field, value, onChange, error, disabled }: WidgetProps) {
  return (
    <TextField
      fullWidth
      size="small"
      label={field.labelKey}
      value={typeof value === 'string' ? value : ''}
      onChange={(event) => onChange(event.target.value)}
      error={Boolean(error)}
      helperText={error}
      disabled={disabled}
      required={field.isRequired}
    />
  );
}
