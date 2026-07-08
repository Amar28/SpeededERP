import FormControl from '@mui/material/FormControl';
import FormHelperText from '@mui/material/FormHelperText';
import InputLabel from '@mui/material/InputLabel';
import MenuItem from '@mui/material/MenuItem';
import Select, { type SelectChangeEvent } from '@mui/material/Select';

import type { WidgetProps } from '../registry';

export function ListWidget({ field, value, onChange, error, disabled }: WidgetProps) {
  const labelId = `field-${field.key}-label`;

  return (
    <FormControl fullWidth size="small" error={Boolean(error)} required={field.isRequired}>
      <InputLabel id={labelId}>{field.labelKey}</InputLabel>
      <Select
        labelId={labelId}
        label={field.labelKey}
        value={typeof value === 'string' ? value : ''}
        onChange={(event: SelectChangeEvent) => onChange(event.target.value)}
        disabled={disabled}
      >
        {field.listOptions.map((option) => (
          <MenuItem key={option} value={option}>
            {option}
          </MenuItem>
        ))}
      </Select>
      {error ? <FormHelperText>{error}</FormHelperText> : null}
    </FormControl>
  );
}
