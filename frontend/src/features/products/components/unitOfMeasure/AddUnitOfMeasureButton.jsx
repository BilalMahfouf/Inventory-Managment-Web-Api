import Button from '@components/Buttons/Button';
import AddUnitOfMeasure from './AddUnitOfMeasure';
import { useState } from 'react';
import { useTranslation } from 'react-i18next';

import i18nKeyContainer from '@shared/lib/i18n/keyContainer';
export default function AddUnitOfMeasureButton() {
  const { t } = useTranslation();
  const [isOpen, setIsOpen] = useState(false);

  return (
    <>
      <Button
        onClick={() => {
          setIsOpen(true);
        }}
      >
        {t(i18nKeyContainer.products.units.buttonAdd)}
      </Button>

      {isOpen && (
        <AddUnitOfMeasure
          isOpen={isOpen}
          onClose={() => setIsOpen(false)}
          onSuccess={() => {}}
        />
      )}
    </>
  );
}
