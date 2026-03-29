import SimpleDataTable from '@components/DataTable/SimpleDataTable';
import React, { useState } from 'react';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import {
  getLocations,
  deleteLocation,
} from '@features/inventory/services/locationApi';
import AddUpdateLocation from './AddUpdateLocation';
import ViewLocation from './ViewLocation';
import { useToast } from '@shared/context/ToastContext';
import ConfirmationDialog from '@components/ui/ConfirmationDialog';
import { useTranslation } from 'react-i18next';
import i18nKeyContainer from '@shared/lib/i18n/keyContainer';
import { queryKeys } from '@shared/lib/queryKeys';
import { formatAppDate } from '@shared/utils/dateFormatter';

const getDefaultColumns = t => [
  {
    accessorKey: 'id',
    header: t(i18nKeyContainer.inventory.locations.table.columns.id),
  },
  {
    accessorKey: 'name',
    header: t(i18nKeyContainer.inventory.locations.table.columns.name),
  },
  {
    accessorKey: 'address',
    header: t(i18nKeyContainer.inventory.locations.table.columns.address),
  },
  {
    accessorKey: 'locationTypeName',
    header: t(i18nKeyContainer.inventory.locations.table.columns.typeName),
  },
  {
    accessorKey: 'createdAt',
    header: t(i18nKeyContainer.inventory.locations.table.columns.createdAt),
    cell: ({ getValue }) => formatAppDate(getValue()),
  },
  {
    accessorKey: 'createdByUserName',
    header: t(i18nKeyContainer.inventory.locations.table.columns.createdBy),
  },
];

export default function LocationDataTable() {
  const { t } = useTranslation();
  const [addEditDialogOpen, setAddEditDialogOpen] = useState(false);
  const [viewDialogOpen, setViewDialogOpen] = useState(false);
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  const [currentLocationId, setCurrentLocationId] = useState(0);
  const { showError, showSuccess } = useToast();
  const queryClient = useQueryClient();

  const { data = null, isLoading } = useQuery({
    queryKey: queryKeys.inventory.locations('list'),
    queryFn: async () => {
      const response = await getLocations();
      return response.success ? response.data : null;
    },
  });

  const deleteMutation = useMutation({
    mutationFn: deleteLocation,
    onSuccess: async response => {
      if (!response.success) {
        showError(
          t(i18nKeyContainer.inventory.locations.table.toasts.deleteFailedTitle),
          response.message ||
            t(
              i18nKeyContainer.inventory.locations.form.toasts
                .loadLocationFailedMessage
            )
        );
        setDeleteDialogOpen(false);
        return;
      }
      showSuccess(
        t(i18nKeyContainer.inventory.locations.table.toasts.deleteSuccessTitle),
        t(i18nKeyContainer.inventory.locations.table.toasts.deleteSuccessMessage, {
          id: currentLocationId,
        })
      );
      setDeleteDialogOpen(false);
      await queryClient.invalidateQueries({
        queryKey: queryKeys.inventory.locations('list'),
      });
    },
  });

  const handleAddNew = () => {
    setCurrentLocationId(0);
    setAddEditDialogOpen(true);
  };

  const handleView = row => {
    setCurrentLocationId(row.id);
    setViewDialogOpen(true);
  };

  const handleEdit = row => {
    setCurrentLocationId(row.id);
    setAddEditDialogOpen(true);
  };

  const handleCloseAddEdit = () => {
    setAddEditDialogOpen(false);
    setCurrentLocationId(0);
  };

  const handleSuccess = async () => {
    await queryClient.invalidateQueries({
      queryKey: queryKeys.inventory.locations('list'),
    });
    await queryClient.invalidateQueries({ queryKey: queryKeys.inventory.summary() });
    setAddEditDialogOpen(false);
    setViewDialogOpen(false);
    setDeleteDialogOpen(false);
  };
  const handleDelete = async () => {
    deleteMutation.mutate(currentLocationId);
  };

  return (
    <>
      <SimpleDataTable
        data={data}
        columns={getDefaultColumns(t)}
        loading={isLoading}
        enableActions={true}
        onView={handleView}
        onEdit={handleEdit}
        onDelete={row => {
          setCurrentLocationId(row.id);
          setDeleteDialogOpen(true);
        }}
        onAddNew={handleAddNew}
      />
      {addEditDialogOpen && (
        <AddUpdateLocation
          isOpen={addEditDialogOpen}
          onClose={handleCloseAddEdit}
          locationId={currentLocationId}
          onSuccess={handleSuccess}
        />
      )}
      {viewDialogOpen && (
        <ViewLocation
          open={viewDialogOpen}
          onOpenChange={setViewDialogOpen}
          locationId={currentLocationId}
        />
      )}
      {deleteDialogOpen && (
        <ConfirmationDialog
          isOpen={deleteDialogOpen}
          title={t(i18nKeyContainer.inventory.locations.table.dialogs.deleteTitle)}
          message={t(i18nKeyContainer.inventory.locations.table.dialogs.deleteMessage, {
            id: currentLocationId,
          })}
          onConfirm={handleDelete}
          onCancel={() => setDeleteDialogOpen(false)}
          onClose={handleSuccess}
        />
      )}
    </>
  );
}
