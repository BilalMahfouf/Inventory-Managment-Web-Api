import { useState } from 'react';
import { Plus } from 'lucide-react';
import Button from '../Buttons/Button';
import AddStockTransfer from './AddStockTransfer';

export default function AddStockTransferButton({ onSuccess }) {
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
        Add Stock Transfer
      </Button>

      <AddStockTransfer
        isOpen={isModalOpen}
        onClose={() => setIsModalOpen(false)}
        onSuccess={handleSuccess}
      />
    </>
  );
}
