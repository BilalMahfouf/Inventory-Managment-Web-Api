import DataTable from '../DataTable/DataTable';
import { useEffect, useState } from 'react';
import { getUnitOfMeasures } from '@services/products/unitOfMeasureService';
import SimpleDataTable from '../DataTable/SimpleDataTable';
import ConfirmationDialog from '../ui/ConfirmationDialog';
import { deleteUnitOfMeasure } from '@/services/products/UnitOfMeasureService';
import { useToast } from '@/context/ToastContext';
import AddUnitOfMeasure from './AddUnitOfMeasure';
import UnitOfMeasureView from './UnitOfMeasureView';

export default function UnitOfMeasureTable() {
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  const [addEditDialogOpen, setAddEditDialogOpen] = useState(false);
  const [viewDialogOpen, setViewDialogOpen] = useState(false);
  const [currentUnitOfMeasureId, setCurrentUnitOfMeasureId] = useState(0);
  const [data, setData] = useState(null);
  const [isLoading, setIsLoading] = useState(false);

  const { showError, showSuccess } = useToast();

  useEffect(() => {
    const fetchUnitOfMeasures = async () => {
      setIsLoading(true);
      try {
        const responseData = await getUnitOfMeasures();
        setData(responseData);
        console.log('Unit of Measures data:', responseData);
      } catch (error) {
        console.error('Error fetching unit of measures:', error);
        showError('Failed to load units of measure');
      } finally {
        setIsLoading(false);
      }
    };

    fetchUnitOfMeasures();
  }, [showError]);

  const fetchUnitOfMeasures = async () => {
    setIsLoading(true);
    try {
      const responseData = await getUnitOfMeasures();
      setData(responseData);
    } catch (error) {
      console.error('Error fetching unit of measures:', error);
      showError('Failed to load units of measure');
    } finally {
      setIsLoading(false);
    }
  };

  const handleView = row => {
    setCurrentUnitOfMeasureId(row.id);
    setViewDialogOpen(true);
  };

  const handleEdit = row => {
    setCurrentUnitOfMeasureId(row.id);
    setAddEditDialogOpen(true);
  };

  const handleDelete = () => {
    deleteUnitOfMeasure(currentUnitOfMeasureId)
      .then(() => {
        setDeleteDialogOpen(false);
        setCurrentUnitOfMeasureId(0);
        fetchUnitOfMeasures(); // Refresh data
        showSuccess(
          `Unit of Measure Deleted`,
          `Unit of measure deleted successfully`
        );
      })
      .catch(error => {
        console.error('Error deleting unit of measure:', error);
        showError('Failed to delete unit of measure');
      });
  };

  const handleAddNew = () => {
    setCurrentUnitOfMeasureId(0);
    setAddEditDialogOpen(true);
  };

  const handleCloseAddEdit = () => {
    setAddEditDialogOpen(false);
    setCurrentUnitOfMeasureId(0);
  };

  const handleSuccess = () => {
    fetchUnitOfMeasures(); // Refresh data after successful add/edit
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
        onAddNew={handleAddNew}
      />
      {deleteDialogOpen && (
        <ConfirmationDialog
          title='Delete Unit of Measure'
          message={`Are you sure you want to delete this unit of measure? This action cannot be undone.`}
          onConfirm={handleDelete}
          onClose={() => setDeleteDialogOpen(false)}
          isOpen={deleteDialogOpen}
        />
      )}
      <AddUnitOfMeasure
        isOpen={addEditDialogOpen}
        onClose={handleCloseAddEdit}
        unitId={currentUnitOfMeasureId}
        onSuccess={handleSuccess}
      />
      <UnitOfMeasureView
        open={viewDialogOpen}
        onOpenChange={setViewDialogOpen}
        unitId={currentUnitOfMeasureId}
      />
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
