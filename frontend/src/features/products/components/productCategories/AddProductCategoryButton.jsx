import Button from '@components/Buttons/Button';
import AddProductCategory from './AddProductCategory';
import { useState } from 'react';
import { useTranslation } from 'react-i18next';

import i18nKeyContainer from '@shared/lib/i18n/keyContainer';

export default function AddProductCategoryButton({ onClose }) {
  const { t } = useTranslation();
  const [isOpen, setIsOpen] = useState(false);

  return (
    <>
      <Button
        onClick={() => {
          setIsOpen(true);
        }}
      >
        {t(i18nKeyContainer.products.categories.buttonAdd)}
      </Button>

      {isOpen && (
        <AddProductCategory
          isOpen={isOpen}
          onClose={() => {
            setIsOpen(false);
            if (onClose) onClose();
          }}
          onSuccess={() => {}}
        />
      )}
    </>
  );
}
