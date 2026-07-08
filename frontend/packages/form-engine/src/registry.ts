import type { ComponentType } from 'react';

import type { FieldDefinition, FieldType } from './types';

/**
 * Props every field widget receives from `FormRenderer`. Widgets are deliberately
 * "dumb": they render an input bound to `value`/`onChange` and know nothing about
 * react-hook-form, validation, or layout — the engine handles all of that.
 */
export interface WidgetProps {
  field: FieldDefinition;
  value: unknown;
  onChange: (value: unknown) => void;
  error?: string;
  disabled?: boolean;
}

export type WidgetComponent = ComponentType<WidgetProps>;

/**
 * The extensibility point called out by architecture section 11: "ajouter un
 * type de champ, c'est ajouter une entrée au registre, jamais toucher au moteur"
 * (adding a field type means adding a registry entry, never touching the engine).
 *
 * `FormRenderer` never special-cases a `FieldType` — it always resolves
 * `registry[field.type]` and renders whatever widget is registered there.
 */
export type FieldWidgetRegistry = Record<FieldType, WidgetComponent>;
