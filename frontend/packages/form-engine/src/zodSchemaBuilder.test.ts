import { describe, expect, it } from 'vitest';

import { buildFieldSchema, buildFormSchema } from './zodSchemaBuilder';
import type { FieldDefinition } from './types';

function field(overrides: Partial<FieldDefinition> = {}): FieldDefinition {
  return {
    id: 'f1',
    key: 'numero_fiscal',
    labelKey: 'labels.numero_fiscal',
    type: 'TextBox',
    isRequired: false,
    order: 0,
    listOptions: [],
    ...overrides,
  };
}

describe('buildFieldSchema', () => {
  it('rejects an empty string for a required TextBox field', () => {
    const schema = buildFieldSchema(field({ isRequired: true }));

    expect(schema.safeParse('').success).toBe(false);
    expect(schema.safeParse('FR123').success).toBe(true);
  });

  it('allows an absent value for a non-required field', () => {
    const schema = buildFieldSchema(field({ isRequired: false }));

    expect(schema.safeParse(undefined).success).toBe(true);
  });

  it('constrains a List field to its declared options', () => {
    const schema = buildFieldSchema(
      field({ type: 'List', listOptions: ['Entreprise', 'Particulier'] }),
    );

    expect(schema.safeParse('Entreprise').success).toBe(true);
    expect(schema.safeParse('Association').success).toBe(false);
  });

  it('rejects a non-numeric value for a Number field', () => {
    const schema = buildFieldSchema(field({ type: 'Number' }));

    expect(schema.safeParse(42).success).toBe(true);
    expect(schema.safeParse('quarante-deux').success).toBe(false);
  });

  it('validates a Date field against the YYYY-MM-DD shape', () => {
    const schema = buildFieldSchema(field({ type: 'Date' }));

    expect(schema.safeParse('2026-07-08').success).toBe(true);
    expect(schema.safeParse('08/07/2026').success).toBe(false);
  });
});

describe('buildFormSchema', () => {
  it('reproduces the architecture section 10 example: numero_fiscal required only for tenant B', () => {
    const fields: FieldDefinition[] = [
      field({ key: 'nom', type: 'TextBox', isRequired: true }),
      field({ key: 'numero_fiscal', type: 'TextBox', isRequired: true }),
      field({
        key: 'secteur',
        type: 'List',
        isRequired: true,
        listOptions: ['Entreprise', 'Particulier'],
      }),
    ];

    const schema = buildFormSchema(fields);

    const valid = schema.safeParse({
      nom: 'Client A',
      numero_fiscal: 'FR123456',
      secteur: 'Entreprise',
    });
    expect(valid.success).toBe(true);

    const missingRequired = schema.safeParse({ nom: 'Client A' });
    expect(missingRequired.success).toBe(false);
  });
});
