import AddUpdateLocation from './AddUpdateLocation';
import Button from '@components/Buttons/Button';
import { Plus } from 'lucide-react';
import { useState } from 'react';
import { useTranslation } from 'react-i18next';
import i18nKeyContainer from '@shared/lib/i18n/keyContainer';

export default function AddLocationButton({ onSuccess }) {
  const { t } = useTranslation();
  const [isDialogOpen, setIsDialogOpen] = useState(false);

  return (
    <>
      <Button onClick={() => setIsDialogOpen(true)}>
        <Plus className='mr-2' />
        {t(i18nKeyContainer.inventory.locations.buttonAdd)}
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
