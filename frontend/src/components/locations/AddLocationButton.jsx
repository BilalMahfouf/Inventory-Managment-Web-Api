import AddUpdateLocation from './AddUpdateLocation';
import Button from '../Buttons/Button';
import { Plus } from 'lucide-react';
import { useState } from 'react';

export default function AddLocationButton({ onSuccess }) {
  const [isDialogOpen, setIsDialogOpen] = useState(false);

  return (
    <>
      <Button onClick={() => setIsDialogOpen(true)}>
        <Plus className='mr-2' />
        Add Location
      </Button>
      <AddUpdateLocation
        isOpen={isDialogOpen}
        onClose={() => setIsDialogOpen(false)}
        onSuccess={onSuccess}
        locationId={0}
      />
    </>
  );
}
