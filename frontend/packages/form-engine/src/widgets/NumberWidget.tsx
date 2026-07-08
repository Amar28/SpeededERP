import TextField from '@mui/material/TextField';

import type { WidgetProps } from '../registry';

export function NumberWidget({ field, value, onChange, error, disabled }: WidgetProps) {
  return (
    <TextField
      fullWidth
      size="small"
      type="number"
      label={field.labelKey}
      value={typeof value === 'number' ? value : (value ?? '')}
      onChange={(event) => {
        const raw = event.target.value;
        onChange(raw === '' ? undefined : Number(raw));
      }}
      error={Boolean(error)}
      helperText={error}
      disabled={disabled}
      required={field.isRequired}
    />
  );
}
