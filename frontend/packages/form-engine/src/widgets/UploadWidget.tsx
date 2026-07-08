import UploadFileOutlinedIcon from '@mui/icons-material/UploadFileOutlined';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import FormHelperText from '@mui/material/FormHelperText';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';

import type { WidgetProps } from '../registry';

/**
 * Simple file input (no drag-and-drop library, per spec — a full dropzone can
 * be swapped in later by registering a different component under 'Upload').
 * Stores the selected file's name/size in form state as a lightweight
 * descriptor; the actual binary upload flow is out of scope for this widget.
 */
export function UploadWidget({ field, value, onChange, error, disabled }: WidgetProps) {
  const fileName =
    value && typeof value === 'object' && 'name' in value ? String(value.name) : undefined;

  return (
    <Stack spacing={0.5}>
      <Typography variant="caption" color="text.secondary">
        {field.labelKey}
        {field.isRequired ? ' *' : ''}
      </Typography>
      <Box>
        <Button
          component="label"
          variant="outlined"
          size="small"
          startIcon={<UploadFileOutlinedIcon />}
          disabled={disabled}
        >
          {fileName ?? 'Choisir un fichier'}
          <input
            type="file"
            hidden
            onChange={(event) => {
              const file = event.target.files?.[0];
              onChange(file ? { name: file.name, size: file.size, type: file.type } : undefined);
            }}
          />
        </Button>
      </Box>
      {error ? <FormHelperText error>{error}</FormHelperText> : null}
    </Stack>
  );
}
