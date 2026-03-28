import React, { useState } from 'react';
import { Plus } from 'lucide-react';
import Button from '@components/Buttons/Button';
import AddUpdateInventory from './AddUpdateInventory';
import { useTranslation } from 'react-i18next';
import i18nKeyContainer from '@shared/lib/i18n/keyContainer';

/**
 * AddInventoryButton Component
 *
 * A button component that opens the AddUpdateInventory dialog.
 * Used to add new inventory records to the system.
 *
 * @param {Object} props - Component props
 * @param {function} props.onSuccess - Optional callback after successful creation
 */
const AddInventoryButton = ({ onSuccess }) => {
  const { t } = useTranslation();
  const [dialogOpen, setDialogOpen] = useState(false);

  const handleSuccess = () => {
    setDialogOpen(false);
    if (onSuccess) {
      onSuccess();
    }
  };

  return (
    <>
      <Button
        onClick={() => setDialogOpen(true)}
        className='cursor-pointer'
        aria-label={t(i18nKeyContainer.inventory.addInventoryButton.ariaLabel)}
      >
        <Plus className='h-4 w-4 mr-2' />
        {t(i18nKeyContainer.inventory.addInventoryButton.label)}
      </Button>

      <AddUpdateInventory
        isOpen={dialogOpen}
        onClose={() => setDialogOpen(false)}
        inventoryId={0}
        onSuccess={handleSuccess}
      />
    </>
  );
};

export default AddInventoryButton;
