/**
 * Mirrors `Core.Metadata`'s field type taxonomy exactly as declared server-side
 * (see `FieldDefinitionEndpoints.cs` / `FieldDefinitionDto`). On the wire this
 * enum is serialized as a raw integer (no `JsonStringEnumConverter` is registered
 * backend-wide) — `FIELD_TYPE_TO_WIRE` / `FIELD_TYPE_FROM_WIRE` below convert
 * between the wire int and this ergonomic string union, which also doubles as
 * the widget registry's key type.
 */
export const FIELD_TYPES = [
  'TextBox',
  'Date',
  'Number',
  'List',
  'Upload',
  'Signature',
  'Map',
  'QrCode',
  'Scanner',
  'Gps',
] as const;

export type FieldType = (typeof FIELD_TYPES)[number];

/** Wire (raw int, per backend enum declaration order) <-> domain string union. */
export const FIELD_TYPE_TO_WIRE: Record<FieldType, number> = FIELD_TYPES.reduce(
  (acc, type, index) => {
    acc[type] = index;
    return acc;
  },
  {} as Record<FieldType, number>,
);

const FIELD_TYPE_FROM_WIRE_ARRAY = FIELD_TYPES;

export function fieldTypeFromWire(wireValue: number): FieldType {
  const type = FIELD_TYPE_FROM_WIRE_ARRAY[wireValue];
  if (!type) {
    throw new Error(`Unknown FieldType wire value: ${wireValue}`);
  }
  return type;
}

export function fieldTypeToWire(type: FieldType): number {
  return FIELD_TYPE_TO_WIRE[type];
}

/** Mirrors `FieldDefinitionDto` (Core.Metadata), with `type` normalized to the string union. */
export interface FieldDefinition {
  id: string;
  key: string;
  labelKey: string;
  type: FieldType;
  isRequired: boolean;
  order: number;
  listOptions: string[];
}

/** A single field placement within a form row (architecture section 11: "sections/rows/columns"). */
export interface FormLayoutColumn {
  id: string;
  /** Matches `FieldDefinition.key`. */
  fieldKey: string;
  /** Grid width out of 12 columns. Defaults to 12 (full width) when omitted. */
  width?: number;
}

export interface FormLayoutRow {
  id: string;
  columns: FormLayoutColumn[];
}

export interface FormLayoutSection {
  id: string;
  /** i18n key or literal title for the section heading. */
  titleKey?: string;
  rows: FormLayoutRow[];
}

/** Parsed shape of `FormDefinitionDto.layoutJson` (opaque string on the wire; see api-client). */
export interface FormLayout {
  sections: FormLayoutSection[];
}

/** Mirrors `FormDefinitionDto` (Core.Metadata), with `layoutJson` parsed into `layout`. */
export interface FormDefinition {
  id: string;
  entityDefinitionId: string;
  name: string;
  layout: FormLayout;
  version: number;
  isPublished: boolean;
}

/** Parses the raw `layoutJson` string returned by `GET /api/v1/metadata/forms/{id}` into a `FormLayout`. */
export function parseFormLayout(layoutJson: string): FormLayout {
  const parsed: unknown = JSON.parse(layoutJson);
  if (typeof parsed !== 'object' || parsed === null || !('sections' in parsed)) {
    throw new Error('Invalid layoutJson: expected an object with a "sections" array.');
  }
  return parsed as FormLayout;
}

/** Serializes a `FormLayout` back into the `layoutJson` string expected by `POST /api/v1/metadata/forms`. */
export function stringifyFormLayout(layout: FormLayout): string {
  return JSON.stringify(layout);
}
