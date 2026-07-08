import { z } from 'zod';

import type { FieldDefinition } from './types';

const REQUIRED_MESSAGE = 'Ce champ est obligatoire';
const INVALID_DATE_MESSAGE = 'Format de date invalide (AAAA-MM-JJ)';

function baseSchemaForType(field: FieldDefinition): z.ZodTypeAny {
  switch (field.type) {
    case 'Number':
      return z.number({ error: 'Veuillez saisir un nombre valide' });
    case 'Date':
      return z.string().regex(/^\d{4}-\d{2}-\d{2}$/, INVALID_DATE_MESSAGE);
    case 'List':
      return field.listOptions.length > 0
        ? z.enum(field.listOptions as [string, ...string[]])
        : z.string();
    case 'Upload':
      return z.object({
        name: z.string(),
        size: z.number(),
        type: z.string(),
      });
    case 'TextBox':
    case 'Signature':
    case 'Map':
    case 'QrCode':
    case 'Scanner':
    case 'Gps':
    default:
      return z.string();
  }
}

/**
 * Builds a per-field zod schema from a `FieldDefinition`, enforcing at least
 * required-ness plus type-appropriate base validation (number vs date-shaped
 * string vs enum-constrained list vs generic string).
 */
export function buildFieldSchema(field: FieldDefinition): z.ZodTypeAny {
  const base = baseSchemaForType(field);

  if (!field.isRequired) {
    return base.optional();
  }

  if (base instanceof z.ZodString) {
    return base.min(1, REQUIRED_MESSAGE);
  }

  return base;
}

export type FormValues = Record<string, unknown>;

/**
 * Builds a full zod object schema for a form from its `FieldDefinition[]`,
 * keyed by `FieldDefinition.key` — this is what gets handed to
 * `@hookform/resolvers/zod`'s `zodResolver` inside `FormRenderer`.
 */
export function buildFormSchema(fields: FieldDefinition[]): z.ZodObject<Record<string, z.ZodTypeAny>> {
  const shape: Record<string, z.ZodTypeAny> = {};
  for (const field of fields) {
    shape[field.key] = buildFieldSchema(field);
  }
  return z.object(shape);
}
