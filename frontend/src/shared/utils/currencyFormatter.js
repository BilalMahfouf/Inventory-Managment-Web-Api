const SUPPORTED_LOCALES = {
  en: 'en-GB',
  fr: 'fr-FR',
  ar: 'ar-DZ',
};

const formatterCache = new Map();

function normalizeLocale(locale = 'en') {
  const shortLocale = String(locale || 'en')
    .toLowerCase()
    .split('-')[0];

  return SUPPORTED_LOCALES[shortLocale] || SUPPORTED_LOCALES.en;
}

export function getDzdCurrencySuffix(locale = 'en') {
  const shortLocale = String(locale || 'en')
    .toLowerCase()
    .split('-')[0];

  return shortLocale === 'ar' ? 'دج' : 'DA';
}

function getFormatter(locale) {
  const normalizedLocale = normalizeLocale(locale);

  if (!formatterCache.has(normalizedLocale)) {
    formatterCache.set(
      normalizedLocale,
      new Intl.NumberFormat(normalizedLocale, {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2,
        numberingSystem: 'latn',
      })
    );
  }

  return formatterCache.get(normalizedLocale);
}

export function formatDzdCurrency(value, options = {}) {
  const { locale = 'en' } = options;
  const amount = Number(value);
  const safeAmount = Number.isFinite(amount) ? amount : 0;
  const formatter = getFormatter(locale);
  const suffix = getDzdCurrencySuffix(locale);

  return `${formatter.format(safeAmount)} ${suffix}`;
}
