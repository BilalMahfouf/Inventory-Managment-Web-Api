import { useEffect, useMemo, useState } from 'react';
import { getUnitOfMeasures } from '@features/products/services/unitOfMeasureApi';
import SimpleDataTable from '@components/DataTable/SimpleDataTable';
import ConfirmationDialog from '@components/ui/ConfirmationDialog';
import { deleteUnitOfMeasure } from '@features/products/services/unitOfMeasureApi';
import { useToast } from '@shared/context/ToastContext';
import AddUnitOfMeasure from './AddUnitOfMeasure';
import UnitOfMeasureView from './UnitOfMeasureView';
import { useTranslation } from 'react-i18next';

import i18nKeyContainer from '@shared/lib/i18n/keyContainer';

export default function UnitOfMeasureTable() {
  const { t } = useTranslation();
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  const [addEditDialogOpen, setAddEditDialogOpen] = useState(false);
  const [viewDialogOpen, setViewDialogOpen] = useState(false);
  const [currentUnitOfMeasureId, setCurrentUnitOfMeasureId] = useState(0);
  const [data, setData] = useState(null);
  const [isLoading, setIsLoading] = useState(false);

  const { showError, showSuccess } = useToast();
  const columns = useMemo(() => getDefaultColumns(t), [t]);

  useEffect(() => {
    const fetchUnitOfMeasures = async () => {
      setIsLoading(true);
      try {
        const responseData = await getUnitOfMeasures();
        setData(responseData);
        console.log('Unit of Measures data:', responseData);
      } catch (error) {
        console.error('Error fetching unit of measures:', error);
        showError(t(i18nKeyContainer.products.units.table.toasts.loadError));
      } finally {
        setIsLoading(false);
      }
    };

    fetchUnitOfMeasures();
  }, [showError, t]);

  const fetchUnitOfMeasures = async () => {
    setIsLoading(true);
    try {
      const responseData = await getUnitOfMeasures();
      setData(responseData);
    } catch (error) {
      console.error('Error fetching unit of measures:', error);
      showError(t(i18nKeyContainer.products.units.table.toasts.loadError));
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
          t(i18nKeyContainer.products.units.table.toasts.deleteSuccessTitle),
          t(i18nKeyContainer.products.units.table.toasts.deleteSuccessMessage)
        );
      })
      .catch(error => {
        console.error('Error deleting unit of measure:', error);
        showError(t(i18nKeyContainer.products.units.table.toasts.deleteError));
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
        columns={columns}
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
          title={t(i18nKeyContainer.products.units.table.dialogs.deleteTitle)}
          message={t(i18nKeyContainer.products.units.table.dialogs.deleteMessage)}
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

const getDefaultColumns = t => [
  {
    accessorKey: 'id',
    header: t(i18nKeyContainer.products.units.table.columns.id),
  },
  {
    accessorKey: 'name',
    header: t(i18nKeyContainer.products.units.table.columns.name),
  },
  {
    accessorKey: 'createdAt',
    header: t(i18nKeyContainer.products.units.table.columns.createdAt),
    cell: ({ getValue }) => new Date(getValue()).toLocaleDateString(),
  },
  {
    accessorKey: 'createdByUserName',
    header: t(i18nKeyContainer.products.units.table.columns.createdBy),
  },
];
