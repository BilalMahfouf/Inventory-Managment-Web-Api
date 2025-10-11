import DataTable from '../DataTable/DataTable';
import { useEffect, useState } from 'react';
import { getUnitOfMeasures } from '@services/products/unitOfMeasureService';
import SimpleDataTable from '../DataTable/SimpleDataTable';
import ConfirmationDialog from '../ui/ConfirmationDialog';
import { deleteUnitOfMeasure } from '@/services/products/UnitOfMeasureService';
import { useToast } from '@/context/ToastContext';

export default function UnitOfMeasureTable() {
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  const [currentUnitOfMeasureId, setCurrentUnitOfMeasureId] = useState(0);
  const [data, setData] = useState(null);
  const [isLoading, setIsLoading] = useState(false);

  const { showError, showSuccess } = useToast();

  useEffect(() => {
    const fetchUnitOfMeasures = async () => {
      setIsLoading(true);
      const responseData = await getUnitOfMeasures();
      setData(responseData);
      setIsLoading(false);
      console.log('Unit of Measures data:', responseData);
    };

    fetchUnitOfMeasures();
  }, [currentUnitOfMeasureId]);

  const handleView = () => {
    console.log('view is clicked');
  };
  const handleEdit = () => {
    console.log('edit is clicked');
  };
  const handleDelete = () => {
    deleteUnitOfMeasure(currentUnitOfMeasureId)
      .then(() => {
        setDeleteDialogOpen(false);
        setCurrentUnitOfMeasureId(0); // Trigger data refresh
        showSuccess(
          `Unit of Measure Deleted`,
          `Unit of measure with id ${currentUnitOfMeasureId} deleted successfully`
        );
      })
      .catch(error => {
        console.error('Error deleting unit of measure:', error);
        showError('Failed to delete unit of measure');
      });
  };

  return (
    <>
      <SimpleDataTable
        data={data}
        columns={defaultColumns}
        loading={isLoading}
        enableActions={true}
        onView={handleView}
        onEdit={handleEdit}
        onDelete={row => {
          setCurrentUnitOfMeasureId(row.id);
          setDeleteDialogOpen(true);
        }}
      />
      {deleteDialogOpen && (
        <ConfirmationDialog
          title='Delete Unit of Measure'
          message={`Are you sure you want to delete unit of measure with id ${currentUnitOfMeasureId}? This action cannot be undone.`}
          onConfirm={handleDelete}
          onClose={() => setDeleteDialogOpen(false)}
          isOpen={deleteDialogOpen}
        />
      )}
    </>
  );
}

const defaultColumns = [
  {
    accessorKey: 'id',
    header: 'ID',
  },
  {
    accessorKey: 'name',
    header: 'Name',
  },
  {
    accessorKey: 'createdAt',
    header: 'Created At',
    cell: ({ getValue }) => new Date(getValue()).toLocaleDateString(),
  },
  {
    accessorKey: 'createdByUserName',
    header: 'Created By',
  },
];
