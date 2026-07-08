/**
 * Mirrors the backend's `Money` value object as it is actually observed on the wire:
 * most CRM endpoints flatten `Money` into a bare `decimal` amount plus a sibling
 * ISO 4217 currency field (e.g. `DevisDto.montant` + `DevisDto.devise`). This shape
 * is provided for call sites (or future endpoints) that DO expose Money as a
 * nested `{ amount, currency }` object.
 */
export interface Money {
  amount: number;
  currency: string;
}

/**
 * Formats a monetary amount using `Intl.NumberFormat`, respecting the given
 * ISO 4217 currency code. Defaults to the `fr-FR` locale grouping/decimal
 * conventions, matching the backend's French domain language.
 */
export function formatMoney(money: Money, locale = 'fr-FR'): string {
  return new Intl.NumberFormat(locale, {
    style: 'currency',
    currency: money.currency,
    currencyDisplay: 'symbol',
  }).format(money.amount);
}

/**
 * Formats an amount + currency pair that arrived as two separate fields
 * (the common shape on the wire today, e.g. `DevisDto.montant` / `DevisDto.devise`).
 */
export function formatAmountWithCurrency(
  amount: number,
  currency: string,
  locale = 'fr-FR',
): string {
  return formatMoney({ amount, currency }, locale);
}
