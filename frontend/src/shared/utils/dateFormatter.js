const SUPPORTED_LOCALES = {
  en: 'en-GB',
  fr: 'fr-FR',
  ar: 'ar-EG',
};

const formatterCache = new Map();

function normalizeLocale(locale = 'en') {
  const shortLocale = String(locale || 'en')
    .toLowerCase()
    .split('-')[0];

  return SUPPORTED_LOCALES[shortLocale] || SUPPORTED_LOCALES.en;
}

function getFormatter(locale, withTime) {
  const normalizedLocale = normalizeLocale(locale);
  const cacheKey = `${normalizedLocale}-${withTime ? 'datetime' : 'date'}`;

  if (!formatterCache.has(cacheKey)) {
    formatterCache.set(
      cacheKey,
      new Intl.DateTimeFormat(normalizedLocale, {
        day: '2-digit',
        month: '2-digit',
        year: 'numeric',
        ...(withTime
          ? {
              hour: '2-digit',
              minute: '2-digit',
              hour12: false,
              hourCycle: 'h23',
            }
          : {}),
        numberingSystem: 'latn',
      })
    );
  }

  return formatterCache.get(cacheKey);
}

function toValidDate(value) {
  if (value === null || value === undefined || value === '') {
    return null;
  }

  const date = value instanceof Date ? value : new Date(value);
  return Number.isNaN(date.getTime()) ? null : date;
}

function getPart(parts, type) {
  return parts.find(part => part.type === type)?.value || null;
}

export function formatAppDate(value, options = {}) {
  const {
    locale = 'en',
    withTime = false,
    fallback = '-',
  } = options;

  const date = toValidDate(value);
  if (!date) {
    return fallback;
  }

  const formatter = getFormatter(locale, withTime);
  const parts = formatter.formatToParts(date);

  const day = getPart(parts, 'day');
  const month = getPart(parts, 'month');
  const year = getPart(parts, 'year');

  if (!day || !month || !year) {
    return fallback;
  }

  const dateOutput = `${day}/${month}/${year}`;

  if (!withTime) {
    return dateOutput;
  }

  const hour = getPart(parts, 'hour');
  const minute = getPart(parts, 'minute');

  if (!hour || !minute) {
    return fallback;
  }

  return `${dateOutput} ${hour}:${minute}`;
}
