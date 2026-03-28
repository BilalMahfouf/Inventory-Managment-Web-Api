import { useMemo } from 'react';
import { useTranslation } from 'react-i18next';

import i18nKeyContainer from '@shared/lib/i18n/keyContainer';

const languageOptions = [
  {
    value: 'en',
    labelKey: i18nKeyContainer.common.languages.english,
  },
  {
    value: 'fr',
    labelKey: i18nKeyContainer.common.languages.french,
  },
  {
    value: 'ar',
    labelKey: i18nKeyContainer.common.languages.arabic,
  },
];

export default function LanguageSwitcher({ className = '' }) {
  const { t, i18n } = useTranslation();

  const selectedLanguage = useMemo(() => {
    const resolvedLanguage = i18n.resolvedLanguage || i18n.language || 'en';
    return resolvedLanguage.toLowerCase().split('-')[0];
  }, [i18n.language, i18n.resolvedLanguage]);

  const handleLanguageChange = event => {
    i18n.changeLanguage(event.target.value);
  };

  return (
    <div className={`flex items-center gap-2 ${className}`}>
      <label htmlFor='app-language' className='sr-only'>
        {t(i18nKeyContainer.layout.topNav.languageSwitcher)}
      </label>
      <select
        id='app-language'
        aria-label={t(i18nKeyContainer.layout.topNav.languageSwitcher)}
        value={selectedLanguage}
        onChange={handleLanguageChange}
        className='h-9 rounded-md border border-gray-200 bg-white px-2 text-sm text-gray-700 focus:outline-none focus:ring-1 focus:ring-blue-500'
      >
        {languageOptions.map(option => (
          <option key={option.value} value={option.value}>
            {t(option.labelKey)}
          </option>
        ))}
      </select>
    </div>
  );
}
