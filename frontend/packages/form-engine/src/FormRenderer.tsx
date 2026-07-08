import { zodResolver } from '@hookform/resolvers/zod';
import Alert from '@mui/material/Alert';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Grid from '@mui/material/Grid2';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import { useEffect, useMemo } from 'react';
import { Controller, useForm } from 'react-hook-form';

import { defaultWidgetRegistry } from './defaultRegistry';
import type { FieldWidgetRegistry } from './registry';
import type { FieldDefinition, FormDefinition } from './types';
import { buildFormSchema, type FormValues } from './zodSchemaBuilder';

export interface FormRendererProps {
  /** The FormDefinition whose `layout` (parsed `layoutJson`) drives section/row/column placement. */
  definition: FormDefinition;
  /** The field catalog this form's layout references, by `key`. */
  fields: FieldDefinition[];
  /** Current form values, keyed by `FieldDefinition.key`. */
  value: FormValues;
  /** Called with the full value object whenever any field changes. */
  onChange: (value: FormValues) => void;
  /** If provided, renders a submit button; called with validated values on submit. */
  onSubmit?: (value: FormValues) => void;
  /**
   * Widget registry to resolve each field's `FieldType` against. Defaults to
   * `defaultWidgetRegistry`. Extend via `{ ...defaultWidgetRegistry, Map: MyWidget }`
   * — this is the sole extension point; the renderer itself never special-cases a type.
   */
  registry?: FieldWidgetRegistry;
  submitLabel?: string;
  disabled?: boolean;
}

/**
 * Renders a `FormDefinition` + `FieldDefinition[]` (the Core.Metadata no-code
 * model) by walking `layout.sections -> rows -> columns` and resolving each
 * column's `fieldKey` to a `FieldDefinition`, then to a widget via the
 * registry. Validation is wired through react-hook-form + a zod schema built
 * dynamically from the field catalog (see `zodSchemaBuilder.ts`).
 */
export function FormRenderer({
  definition,
  fields,
  value,
  onChange,
  onSubmit,
  registry = defaultWidgetRegistry,
  submitLabel = 'Valider',
  disabled = false,
}: FormRendererProps) {
  const fieldsByKey = useMemo(() => new Map(fields.map((field) => [field.key, field])), [fields]);
  const schema = useMemo(() => buildFormSchema(fields), [fields]);

  const {
    control,
    handleSubmit,
    watch,
    formState: { errors, isSubmitted },
  } = useForm<FormValues>({
    resolver: zodResolver(schema),
    defaultValues: value,
    mode: 'onBlur',
  });

  useEffect(() => {
    const subscription = watch((values) => onChange(values as FormValues));
    return () => subscription.unsubscribe();
  }, [watch, onChange]);

  const hasSubmitErrors = isSubmitted && Object.keys(errors).length > 0;

  return (
    <Box
      component="form"
      noValidate
      onSubmit={onSubmit ? handleSubmit((data) => onSubmit(data)) : undefined}
    >
      <Stack spacing={4}>
        {definition.layout.sections.map((section) => (
          <Stack key={section.id} spacing={2}>
            {section.titleKey ? (
              <Typography variant="subtitle1" sx={{ fontWeight: 600 }}>
                {section.titleKey}
              </Typography>
            ) : null}
            <Stack spacing={2}>
              {section.rows.map((row) => (
                <Grid container spacing={2} key={row.id}>
                  {row.columns.map((column) => {
                    const field = fieldsByKey.get(column.fieldKey);

                    if (!field) {
                      return (
                        <Grid key={column.id} size={{ xs: 12, sm: column.width ?? 12 }}>
                          <Alert severity="warning">Champ inconnu : {column.fieldKey}</Alert>
                        </Grid>
                      );
                    }

                    const Widget = registry[field.type];
                    const fieldError = errors[field.key];

                    return (
                      <Grid key={column.id} size={{ xs: 12, sm: column.width ?? 12 }}>
                        <Controller
                          name={field.key}
                          control={control}
                          render={({ field: rhfField }) => (
                            <Widget
                              field={field}
                              value={rhfField.value}
                              onChange={rhfField.onChange}
                              disabled={disabled}
                              error={
                                fieldError && typeof fieldError.message === 'string'
                                  ? fieldError.message
                                  : undefined
                              }
                            />
                          )}
                        />
                      </Grid>
                    );
                  })}
                </Grid>
              ))}
            </Stack>
          </Stack>
        ))}
        {onSubmit ? (
          <Box>
            <Button type="submit" variant="contained" disabled={disabled}>
              {submitLabel}
            </Button>
            {hasSubmitErrors ? (
              <Alert severity="error" sx={{ mt: 2 }}>
                Veuillez corriger les erreurs du formulaire.
              </Alert>
            ) : null}
          </Box>
        ) : null}
      </Stack>
    </Box>
  );
}
