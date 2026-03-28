import React, { useState } from 'react';
import { Plus } from 'lucide-react';
import Button from '@components/Buttons/Button';
import AddUpdateInventory from './AddUpdateInventory';

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
        aria-label='Add new inventory'
      >
        <Plus className='h-4 w-4 mr-2' />
        Add Inventory
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
