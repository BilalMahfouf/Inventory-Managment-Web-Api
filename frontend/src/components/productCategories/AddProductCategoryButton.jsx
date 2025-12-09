import Button from '../Buttons/Button';
import AddProductCategory from './AddProductCategory';
import { useState } from 'react';

export default function AddProductCategoryButton({ onClose }) {
  const [isOpen, setIsOpen] = useState(false);

  return (
    <>
      <Button
        onClick={() => {
          setIsOpen(true);
        }}
      >
        Add Product Category
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
