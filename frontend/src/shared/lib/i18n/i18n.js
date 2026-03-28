import i18n from 'i18next';
import { initReactI18next } from 'react-i18next';

import en from './locales/en.json';
import fr from './locales/fr.json';
import ar from './locales/ar.json';

const LANGUAGE_STORAGE_KEY = 'ims_language';
const supportedLanguages = ['en', 'fr', 'ar'];

function normalizeLanguage(value = '') {
  return value.toLowerCase().split('-')[0];
}

function getStoredLanguage() {
  if (typeof window === 'undefined') {
    return null;
  }

  return window.localStorage.getItem(LANGUAGE_STORAGE_KEY);
}

function getBrowserLanguage() {
  if (typeof navigator === 'undefined') {
    return 'en';
  }

  return normalizeLanguage(navigator.language || 'en');
}

function getInitialLanguage() {
  const storedLanguage = normalizeLanguage(getStoredLanguage() || '');

  if (supportedLanguages.includes(storedLanguage)) {
    return storedLanguage;
  }

  const browserLanguage = getBrowserLanguage();
  if (supportedLanguages.includes(browserLanguage)) {
    return browserLanguage;
  }

  return 'en';
}

function applyLanguageToDocument(language) {
  if (typeof document === 'undefined') {
    return;
  }

  const normalizedLanguage = normalizeLanguage(language || 'en');
  document.documentElement.lang = normalizedLanguage;
  document.documentElement.setAttribute('dir', 'ltr');

  if (document.body) {
    document.body.setAttribute('dir', 'ltr');
    document.body.style.direction = 'ltr';
  }
}

function persistLanguage(language) {
  if (typeof window === 'undefined') {
    return;
  }

  window.localStorage.setItem(LANGUAGE_STORAGE_KEY, normalizeLanguage(language));
}

const initialLanguage = getInitialLanguage();

i18n.use(initReactI18next).init({
  resources: {
    en: { translation: en },
    fr: { translation: fr },
    ar: { translation: ar },
  },
  lng: initialLanguage,
  fallbackLng: 'en',
  supportedLngs: supportedLanguages,
  load: 'languageOnly',
  interpolation: {
    escapeValue: false,
  },
  returnNull: false,
});

applyLanguageToDocument(initialLanguage);
persistLanguage(initialLanguage);

i18n.on('languageChanged', newLanguage => {
  applyLanguageToDocument(newLanguage);
  persistLanguage(newLanguage);
});

export default i18n;
