import Alert from '@mui/material/Alert';
import Paper from '@mui/material/Paper';
import Typography from '@mui/material/Typography';
import { FormRenderer, type FieldDefinition, type FormDefinition, type FormValues } from '@speedederp/form-engine';
import { useState } from 'react';
import { useTranslation } from 'react-i18next';

/**
 * Demonstrates the no-code engine end-to-end (architecture sections 10/11) with a
 * hand-authored fixture reproducing the doc's exact example: "Numéro fiscal" and
 * "Secteur" are tenant-B-only custom fields sitting next to the native Nom/Adresse
 * fields — same FormRenderer, same widget registry, zero code change between tenants.
 * In production this fixture is replaced by a live
 * `GET /api/v1/metadata/forms/{id}` + `GET /api/v1/metadata/entities/{id}/fields`
 * pair (see @speedederp/api-client's MetadataClient).
 */
const demoFields: FieldDefinition[] = [
  { id: 'f-nom', key: 'nom', labelKey: 'Raison sociale', type: 'TextBox', isRequired: true, order: 0, listOptions: [] },
  { id: 'f-adresse', key: 'adresse', labelKey: 'Adresse', type: 'TextBox', isRequired: true, order: 1, listOptions: [] },
  {
    id: 'f-numero-fiscal',
    key: 'numero_fiscal',
    labelKey: 'Numéro fiscal',
    type: 'TextBox',
    isRequired: true,
    order: 2,
    listOptions: [],
  },
  {
    id: 'f-secteur',
    key: 'secteur',
    labelKey: 'Secteur',
    type: 'List',
    isRequired: true,
    order: 3,
    listOptions: ['Entreprise', 'Particulier', 'Association'],
  },
  {
    id: 'f-date-visite',
    key: 'date_visite',
    labelKey: 'Date de visite',
    type: 'Date',
    isRequired: false,
    order: 4,
    listOptions: [],
  },
];

const demoDefinition: FormDefinition = {
  id: 'demo-form',
  entityDefinitionId: 'demo-entity',
  name: 'Fiche client — démonstration',
  version: 1,
  isPublished: false,
  layout: {
    sections: [
      {
        id: 'section-identite',
        titleKey: 'Identité',
        rows: [
          { id: 'row-1', columns: [{ id: 'c1', fieldKey: 'nom', width: 6 }, { id: 'c2', fieldKey: 'adresse', width: 6 }] },
          {
            id: 'row-2',
            columns: [
              { id: 'c3', fieldKey: 'numero_fiscal', width: 6 },
              { id: 'c4', fieldKey: 'secteur', width: 6 },
            ],
          },
          { id: 'row-3', columns: [{ id: 'c5', fieldKey: 'date_visite', width: 6 }] },
        ],
      },
    ],
  },
};

export function FormBuilderDemoPage() {
  const { t } = useTranslation();
  const [values, setValues] = useState<FormValues>({});
  const [submitted, setSubmitted] = useState<FormValues | null>(null);

  return (
    <Paper variant="outlined" sx={{ p: 4, maxWidth: 640 }}>
      <Typography variant="h5" component="h1" gutterBottom>
        {t('forms.demo.title')}
      </Typography>
      <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
        {t('forms.demo.subtitle')}
      </Typography>

      {submitted ? <Alert severity="success" sx={{ mb: 2 }}>{t('forms.demo.success')}</Alert> : null}

      <FormRenderer
        definition={demoDefinition}
        fields={demoFields}
        value={values}
        onChange={setValues}
        onSubmit={setSubmitted}
        submitLabel={t('forms.demo.submit')}
      />
    </Paper>
  );
}
