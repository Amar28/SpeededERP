/**
 * Formats an ISO 8601 date/datetime string (as returned by the backend, e.g.
 * `DateOnly`/`DateTime` serialized by System.Text.Json) into a localized display string.
 */
export function formatDate(isoDate: string, locale = 'fr-FR'): string {
  const date = new Date(isoDate);
  if (Number.isNaN(date.getTime())) return isoDate;
  return new Intl.DateTimeFormat(locale, { dateStyle: 'medium' }).format(date);
}

export function formatDateTime(isoDateTime: string, locale = 'fr-FR'): string {
  const date = new Date(isoDateTime);
  if (Number.isNaN(date.getTime())) return isoDateTime;
  return new Intl.DateTimeFormat(locale, { dateStyle: 'medium', timeStyle: 'short' }).format(
    date,
  );
}

/** Converts a `Date` to the `yyyy-MM-dd` string shape expected by `DateOnly`-typed API fields. */
export function toDateOnlyString(date: Date): string {
  return date.toISOString().slice(0, 10);
}
