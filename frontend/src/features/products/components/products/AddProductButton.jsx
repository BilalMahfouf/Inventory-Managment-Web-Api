import { useState } from 'react';
import { useTranslation } from 'react-i18next';

import Button from '@components/Buttons/Button';
import i18nKeyContainer from '@shared/lib/i18n/keyContainer';
import AddProduct from './AddProduct';

export default function AddProductButton() {
  const { t } = useTranslation();
  const [isAddModalOpen, setIsAddModalOpen] = useState(false);

  return (
    <>
      <Button
        children={t(i18nKeyContainer.products.page.addProduct)}
        onClick={() => {
          setIsAddModalOpen(true);
          console.log(`is Add Modal Open: ${isAddModalOpen}`);
        }}
      />
      <AddProduct
        isOpen={isAddModalOpen}
        onClose={() => setIsAddModalOpen(false)}
      />
    </>
  );
}
