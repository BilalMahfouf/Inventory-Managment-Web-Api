import Button from '../Buttons/Button';
import AddUnitOfMeasure from './AddUnitOfMeasure';
import { useState } from 'react';
export default function AddUnitOfMeasureButton() {
  const [isOpen, setIsOpen] = useState(false);

  return (
    <>
      <Button
        onClick={() => {
          setIsOpen(true);
        }}
      >
        Add Unit of Measure
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
