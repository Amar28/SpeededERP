import i18next, { type i18n as I18nInstance } from 'i18next';
import LanguageDetector from 'i18next-browser-languagedetector';
import { initReactI18next } from 'react-i18next';

import en from './locales/en/common.json';
import fr from './locales/fr/common.json';

export const SUPPORTED_LANGUAGES = ['fr', 'en'] as const;
export type SupportedLanguage = (typeof SUPPORTED_LANGUAGES)[number];

export const DEFAULT_LANGUAGE: SupportedLanguage = 'fr';

export interface CreateI18nOptions {
  /** Namespace resources are always registered under 'common'. */
  debug?: boolean;
}

/**
 * Creates and initializes an isolated i18next instance for a SpeededERP app.
 * FR is the default locale (the backend's domain language), EN is the secondary locale.
 */
export function createI18n(options: CreateI18nOptions = {}): I18nInstance {
  const instance = i18next.createInstance();

  void instance
    .use(LanguageDetector)
    .use(initReactI18next)
    .init({
      resources: {
        fr: { common: fr },
        en: { common: en },
      },
      fallbackLng: DEFAULT_LANGUAGE,
      supportedLngs: SUPPORTED_LANGUAGES as unknown as string[],
      defaultNS: 'common',
      ns: ['common'],
      debug: options.debug ?? false,
      interpolation: {
        escapeValue: false,
      },
      detection: {
        order: ['localStorage', 'navigator', 'htmlTag'],
        caches: ['localStorage'],
      },
    });

  return instance;
}

export type { I18nInstance };
export { useTranslation, Trans, I18nextProvider } from 'react-i18next';
