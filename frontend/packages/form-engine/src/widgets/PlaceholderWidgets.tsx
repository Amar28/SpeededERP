import Alert from '@mui/material/Alert';
import Stack from '@mui/material/Stack';
import TextField from '@mui/material/TextField';
import Typography from '@mui/material/Typography';
import type { SvgIconComponent } from '@mui/icons-material';
import DrawOutlinedIcon from '@mui/icons-material/DrawOutlined';
import MapOutlinedIcon from '@mui/icons-material/MapOutlined';
import QrCode2OutlinedIcon from '@mui/icons-material/QrCode2Outlined';
import DocumentScannerOutlinedIcon from '@mui/icons-material/DocumentScannerOutlined';
import GpsFixedOutlinedIcon from '@mui/icons-material/GpsFixedOutlined';

import type { WidgetComponent, WidgetProps } from '../registry';

/**
 * Factory for the field types that don't have a full implementation yet
 * (Signature/Map/QrCode/Scanner/Gps — all require a native capability or a
 * heavier third-party library: signature_pad, maplibre-gl, a QR/barcode
 * decoder, device camera/GPS access). Each still satisfies the full
 * `WidgetProps` contract: it renders something usable and calls `onChange`
 * with a string value, so the registry has all 10 `FieldType` keys populated
 * and consumers can see — and override — the complete extension surface.
 */
function createPlaceholderWidget(options: {
  Icon: SvgIconComponent;
  todoLabel: string;
  inputPlaceholder: string;
}): WidgetComponent {
  const { Icon, todoLabel, inputPlaceholder } = options;

  function PlaceholderWidget({ field, value, onChange, error, disabled }: WidgetProps) {
    return (
      <Stack spacing={1}>
        <Alert severity="info" icon={<Icon fontSize="inherit" />} sx={{ py: 0.5 }}>
          <Typography variant="caption">{todoLabel}</Typography>
        </Alert>
        <TextField
          fullWidth
          size="small"
          label={field.labelKey}
          placeholder={inputPlaceholder}
          value={typeof value === 'string' ? value : ''}
          onChange={(event) => onChange(event.target.value)}
          error={Boolean(error)}
          helperText={error}
          disabled={disabled}
          required={field.isRequired}
        />
      </Stack>
    );
  }

  PlaceholderWidget.displayName = `PlaceholderWidget(${todoLabel})`;
  return PlaceholderWidget;
}

export const SignatureWidget = createPlaceholderWidget({
  Icon: DrawOutlinedIcon,
  todoLabel: 'Signature capture — TODO: wire signature_pad',
  inputPlaceholder: 'Signature (base64) — saisie manuelle temporaire',
});

export const MapWidget = createPlaceholderWidget({
  Icon: MapOutlinedIcon,
  todoLabel: 'Map picker — TODO: wire maplibre-gl',
  inputPlaceholder: 'Coordonnées (lat,lng) — saisie manuelle temporaire',
});

export const QrCodeWidget = createPlaceholderWidget({
  Icon: QrCode2OutlinedIcon,
  todoLabel: 'QR code capture — TODO: wire a QR/barcode decoder + camera access',
  inputPlaceholder: 'Valeur du QR code — saisie manuelle temporaire',
});

export const ScannerWidget = createPlaceholderWidget({
  Icon: DocumentScannerOutlinedIcon,
  todoLabel: 'Document scanner — TODO: wire device camera / scanner integration',
  inputPlaceholder: 'Référence du document scanné — saisie manuelle temporaire',
});

export const GpsWidget = createPlaceholderWidget({
  Icon: GpsFixedOutlinedIcon,
  todoLabel: 'GPS capture — TODO: wire navigator.geolocation',
  inputPlaceholder: 'Coordonnées GPS — saisie manuelle temporaire',
});
