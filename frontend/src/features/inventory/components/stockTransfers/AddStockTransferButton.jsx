import { useState } from 'react';
import { Plus } from 'lucide-react';
import Button from '@components/Buttons/Button';
import AddStockTransfer from './AddStockTransfer';
import { useTranslation } from 'react-i18next';
import i18nKeyContainer from '@shared/lib/i18n/keyContainer';

export default function AddStockTransferButton({ onSuccess }) {
  const { t } = useTranslation();
  const [isModalOpen, setIsModalOpen] = useState(false);

  const handleSuccess = data => {
    setIsModalOpen(false);
    if (onSuccess) {
      onSuccess(data);
    }
  };

  return (
    <>
      <Button onClick={() => setIsModalOpen(true)} LeftIcon={Plus}>
        {t(i18nKeyContainer.inventory.stockTransfers.buttonAdd)}
      </Button>

      <AddStockTransfer
        isOpen={isModalOpen}
        onClose={() => setIsModalOpen(false)}
        onSuccess={handleSuccess}
      />
    </>
  );
}
