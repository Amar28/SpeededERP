import type { FieldWidgetRegistry } from './registry';
import { DateWidget } from './widgets/DateWidget';
import { ListWidget } from './widgets/ListWidget';
import { NumberWidget } from './widgets/NumberWidget';
import {
  GpsWidget,
  MapWidget,
  QrCodeWidget,
  ScannerWidget,
  SignatureWidget,
} from './widgets/PlaceholderWidgets';
import { TextBoxWidget } from './widgets/TextBoxWidget';
import { UploadWidget } from './widgets/UploadWidget';

/**
 * Default widget registry covering all 10 `FieldType` values. Consumers
 * extend/override it with plain object spread:
 * ```ts
 * const myRegistry = { ...defaultWidgetRegistry, Map: MyMapLibreWidget };
 * ```
 */
export const defaultWidgetRegistry: FieldWidgetRegistry = {
  TextBox: TextBoxWidget,
  Date: DateWidget,
  Number: NumberWidget,
  List: ListWidget,
  Upload: UploadWidget,
  Signature: SignatureWidget,
  Map: MapWidget,
  QrCode: QrCodeWidget,
  Scanner: ScannerWidget,
  Gps: GpsWidget,
};
