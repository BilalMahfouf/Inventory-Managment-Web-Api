import { useState } from 'react';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import DataTable from '@components/DataTable/DataTable';
import useServerSideDataTable from '@shared/hooks/useServerSideDataTable';
import {
  getAllInventory,
  deleteInventoryById,
} from '@features/inventory/services/inventoryApi';
import { CheckCircle, AlertTriangle, XCircle } from 'lucide-react';
import AddUpdateInventory from './AddUpdateInventory';
import ViewInventoryDialog from './ViewInventoryDialog';
import ConfirmationDialog from '@components/ui/ConfirmationDialog';
import { useToast } from '@shared/context/ToastContext';
import { useTranslation } from 'react-i18next';
import i18nKeyContainer from '@shared/lib/i18n/keyContainer';
import { queryKeys } from '@shared/lib/queryKeys';
import { formatDzdCurrency } from '@shared/utils/currencyFormatter';

const getLocalizedInventoryStatus = (status, t) => {
  if (status === 'In Stock') {
    return t(i18nKeyContainer.inventory.shared.status.inStock);
  }

  if (status === 'Low Stock') {
    return t(i18nKeyContainer.inventory.shared.status.lowStock);
  }

  if (status === 'Out of Stock') {
    return t(i18nKeyContainer.inventory.shared.status.outOfStock);
  }

  return status;
};

const getDefaultColumns = (t, locale) => [
  {
    accessorKey: 'product',
    header: t(i18nKeyContainer.inventory.inventoryTable.columns.product),
    cell: ({ row }) => (
      <div>
        <div className='font-medium'>{row.original.product}</div>
        <div className='text-sm text-gray-500'>{row.original.sku}</div>
      </div>
    ),
  },
  {
    accessorKey: 'location',
    header: t(i18nKeyContainer.inventory.inventoryTable.columns.location),
  },
  {
    accessorKey: 'quantity',
    header: t(i18nKeyContainer.inventory.inventoryTable.columns.quantity),
    cell: ({ getValue }) => `${getValue().toFixed(2)}`,
  },
  {
    accessorKey: 'reorder',
    header: t(i18nKeyContainer.inventory.inventoryTable.columns.reorder),
    cell: ({ getValue }) => `${getValue().toFixed(2)}`,
  },

  {
    accessorKey: 'max',
    header: t(i18nKeyContainer.inventory.inventoryTable.columns.max),
  },
  {
    accessorKey: 'status',
    header: t(i18nKeyContainer.inventory.inventoryTable.columns.status),
    cell: ({ getValue }) => {
      const status = getValue();
      const isInStock = status === 'In Stock';
      const isLowStock = status === 'Low Stock';
      const isOutOfStock = status === 'Out of Stock';

      return (
        <span
          className={`inline-flex items-center gap-1 px-3 py-1 rounded-full text-xs font-medium transition-colors ${
            isInStock
              ? 'bg-green-100 text-green-700 hover:bg-green-700 hover:text-white'
              : isLowStock
                ? 'bg-yellow-100 text-yellow-700 hover:bg-yellow-600 hover:text-white'
                : 'bg-red-100 text-red-700 hover:bg-red-700 hover:text-white'
          }`}
        >
          {isInStock && <CheckCircle className='w-3 h-3' />}
          {isLowStock && <AlertTriangle className='w-3 h-3' />}
          {isOutOfStock && <XCircle className='w-3 h-3' />}
          {getLocalizedInventoryStatus(status, t)}
        </span>
      );
    },
  },
  {
    accessorKey: 'potentialProfit',
    header: t(
      i18nKeyContainer.inventory.inventoryTable.columns.potentialProfit
    ),
    cell: ({ getValue }) => formatDzdCurrency(getValue(), { locale }),
  },
];

export default function InventoryDataTable() {
  const { t, i18n } = useTranslation();
  const activeLocale = i18n.resolvedLanguage || i18n.language || 'en';
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  const [currentInventoryId, setCurrentInventoryId] = useState(0);
  const [viewDialogOpen, setViewDialogOpen] = useState(false);
  const [editDialogOpen, setEditDialogOpen] = useState(false);
  const { showSuccess, showError } = useToast();
  const queryClient = useQueryClient();

  const deleteMutation = useMutation({
    mutationFn: deleteInventoryById,
    onSuccess: async response => {
      if (response.success) {
        setDeleteDialogOpen(false);
        showSuccess(
          t(
            i18nKeyContainer.inventory.inventoryTable.toasts.deleteSuccessTitle
          ),
          t(
            i18nKeyContainer.inventory.inventoryTable.toasts
              .deleteSuccessMessage,
            {
              id: currentInventoryId,
            }
          )
        );
        await queryClient.invalidateQueries({
          queryKey: queryKeys.inventory.all,
        });
        return;
      }

      showError(
        t(i18nKeyContainer.inventory.inventoryTable.toasts.deleteErrorTitle),
        t(i18nKeyContainer.inventory.inventoryTable.toasts.deleteErrorMessage, {
          error: response.error,
        })
      );
      setDeleteDialogOpen(false);
    },
  });

  const handleDelete = async () => {
    deleteMutation.mutate(currentInventoryId);
  };

  const handleView = row => {
    setCurrentInventoryId(row.id);
    setViewDialogOpen(true);
  };
  const handleEdit = row => {
    console.log(row.product);
    console.log(row.id);
    setCurrentInventoryId(row.id);
    setEditDialogOpen(true);
  };

  const fetchInventories = async ({
    page,
    pageSize,
    search,
    sortOrder,
    sortColumn,
  }) => {
    const response = await getAllInventory({
      page: page,
      pageSize: pageSize,
      sortColumn: sortColumn,
      sortOrder: sortOrder,
      search: search,
    });

    return response.data;
  };

  const tableProps = useServerSideDataTable(fetchInventories, {
    queryKey: queryKeys.inventory.table('catalog'),
  });

  return (
    <>
      <DataTable
        data={tableProps.data}
        columns={getDefaultColumns(t, activeLocale)}
        totalRows={tableProps.totalRows}
        pageIndex={tableProps.pageIndex}
        pageSize={tableProps.pageSize}
        onPageChange={tableProps.onPageChange}
        onPageSizeChange={tableProps.onPageSizeChange}
        onSortingChange={tableProps.onSortingChange}
        onFilterChange={tableProps.onFilterChange}
        loading={tableProps.loading}
        onView={handleView}
        onEdit={handleEdit}
        onDelete={row => {
          setCurrentInventoryId(row.id);
          setDeleteDialogOpen(true);
        }}
      />

      {viewDialogOpen && (
        <ViewInventoryDialog
          open={viewDialogOpen}
          onOpenChange={setViewDialogOpen}
          inventoryId={currentInventoryId}
        />
      )}
      {editDialogOpen && (
        <AddUpdateInventory
          isOpen={editDialogOpen}
          onClose={() => setEditDialogOpen(false)}
          inventoryId={currentInventoryId}
          onSuccess={() => {
            queryClient.invalidateQueries({
              queryKey: queryKeys.inventory.all,
            });
          }}
        />
      )}
      {deleteDialogOpen && (
        <ConfirmationDialog
          isOpen={deleteDialogOpen}
          onClose={() => setDeleteDialogOpen(false)}
          title={t(
            i18nKeyContainer.inventory.inventoryTable.dialogs.deleteTitle
          )}
          message={t(
            i18nKeyContainer.inventory.inventoryTable.dialogs.deleteMessage
          )}
          confirmText={t(
            i18nKeyContainer.inventory.inventoryTable.dialogs.deleteConfirmText
          )}
          onConfirm={handleDelete}
        />
      )}
    </>
  );
}
