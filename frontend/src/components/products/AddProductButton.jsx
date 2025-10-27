import { useState } from 'react';
import Button from '@components/Buttons/Button';
import AddProduct from './AddProduct';

export default function AddProductButton() {
  const [isAddModalOpen, setIsAddModalOpen] = useState(false);

  return (
    <>
      <Button
        children='Add Product'
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
