import FormHelperText from '@mui/material/FormHelperText';
import { AdapterDateFns } from '@mui/x-date-pickers/AdapterDateFnsV3';
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';
import { DatePicker } from '@mui/x-date-pickers/DatePicker';
import { fr } from 'date-fns/locale';

import type { WidgetProps } from '../registry';

/** Wraps its own `LocalizationProvider` so consumers don't need to set one up globally. */
export function DateWidget({ field, value, onChange, error, disabled }: WidgetProps) {
  const dateValue = typeof value === 'string' && value ? new Date(value) : null;

  return (
    <LocalizationProvider dateAdapter={AdapterDateFns} adapterLocale={fr}>
      <DatePicker
        label={field.labelKey}
        value={dateValue}
        onChange={(newDate: Date | null) => {
          onChange(newDate ? newDate.toISOString().slice(0, 10) : undefined);
        }}
        disabled={disabled}
        slotProps={{
          textField: {
            fullWidth: true,
            size: 'small',
            error: Boolean(error),
            required: field.isRequired,
          },
        }}
      />
      {error ? <FormHelperText error>{error}</FormHelperText> : null}
    </LocalizationProvider>
  );
}
