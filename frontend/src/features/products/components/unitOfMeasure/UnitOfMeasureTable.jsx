import { useMemo, useState } from 'react';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { getUnitOfMeasures } from '@features/products/services/unitOfMeasureApi';
import SimpleDataTable from '@components/DataTable/SimpleDataTable';
import ConfirmationDialog from '@components/ui/ConfirmationDialog';
import { deleteUnitOfMeasure } from '@features/products/services/unitOfMeasureApi';
import { useToast } from '@shared/context/ToastContext';
import AddUnitOfMeasure from './AddUnitOfMeasure';
import UnitOfMeasureView from './UnitOfMeasureView';
import { useTranslation } from 'react-i18next';

import i18nKeyContainer from '@shared/lib/i18n/keyContainer';
import { queryKeys } from '@shared/lib/queryKeys';
import { formatAppDate } from '@shared/utils/dateFormatter';

export default function UnitOfMeasureTable() {
  const { t } = useTranslation();
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  const [addEditDialogOpen, setAddEditDialogOpen] = useState(false);
  const [viewDialogOpen, setViewDialogOpen] = useState(false);
  const [currentUnitOfMeasureId, setCurrentUnitOfMeasureId] = useState(0);

  const { showError, showSuccess } = useToast();
  const queryClient = useQueryClient();
  const columns = useMemo(() => getDefaultColumns(t), [t]);

  const { data = [], isLoading } = useQuery({
    queryKey: queryKeys.products.unitOfMeasure(),
    queryFn: getUnitOfMeasures,
  });

  const deleteMutation = useMutation({
    mutationFn: deleteUnitOfMeasure,
    onSuccess: async () => {
      setDeleteDialogOpen(false);
      setCurrentUnitOfMeasureId(0);
      showSuccess(
        t(i18nKeyContainer.products.units.table.toasts.deleteSuccessTitle),
        t(i18nKeyContainer.products.units.table.toasts.deleteSuccessMessage)
      );
      await queryClient.invalidateQueries({
        queryKey: queryKeys.products.unitOfMeasure(),
      });
    },
    onError: () => {
      showError(t(i18nKeyContainer.products.units.table.toasts.deleteError));
    },
  });

  const handleView = row => {
    setCurrentUnitOfMeasureId(row.id);
    setViewDialogOpen(true);
  };

  const handleEdit = row => {
    setCurrentUnitOfMeasureId(row.id);
    setAddEditDialogOpen(true);
  };

  const handleDelete = () => {
    deleteMutation.mutate(currentUnitOfMeasureId);
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
    queryClient.invalidateQueries({
      queryKey: queryKeys.products.unitOfMeasure(),
    });
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
    cell: ({ getValue }) => formatAppDate(getValue()),
  },
  {
    accessorKey: 'createdByUserName',
    header: t(i18nKeyContainer.products.units.table.columns.createdBy),
  },
];
