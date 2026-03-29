import { useToast } from '@shared/context/ToastContext';
import useServerSideDataTable from '@shared/hooks/useServerSideDataTable';
import { useState } from 'react';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import {
  deleteCustomerById,
  getCustomers,
} from '@features/customers/services/customerApi';
import DataTable from '@components/DataTable/DataTable';
import AddUpdateCustomer from './AddUpdateCustomer';
import ConfirmationDialog from '@components/ui/ConfirmationDialog';
import { useTranslation } from 'react-i18next';
import i18nKeyContainer from '@shared/lib/i18n/keyContainer';
import { queryKeys } from '@shared/lib/queryKeys';

const getStatusLabel = (isActive, t) =>
  isActive
    ? t(i18nKeyContainer.customers.shared.status.active)
    : t(i18nKeyContainer.customers.shared.status.inactive);

const getDefaultColumns = t => [
  {
    accessorKey: 'name',
    header: t(i18nKeyContainer.customers.table.columns.customer),
    cell: ({ row }) => (
      <div>
        <div className='font-medium'>{row.original.name}</div>
        <div className='text-sm text-gray-500'>{row.original.email}</div>
      </div>
    ),
  },
  {
    accessorKey: 'phone',
    header: t(i18nKeyContainer.customers.table.columns.phone),
  },
  {
    accessorKey: 'customerCategoryName',
    header: t(i18nKeyContainer.customers.table.columns.category),
  },
  {
    accessorKey: 'totalOrders',
    header: t(i18nKeyContainer.customers.table.columns.totalOrders),
    cell: ({ getValue }) => `${Number(getValue() || 0).toFixed(2)}`,
  },

  {
    accessorKey: 'totalSpent',
    header: t(i18nKeyContainer.customers.table.columns.totalSpent),
    cell: ({ getValue }) => `$${Number(getValue() || 0).toFixed(2)}`,
  },
  {
    accessorKey: 'isActive',
    header: t(i18nKeyContainer.customers.table.columns.isActive),
    cell: ({ getValue }) => {
      const isActive = Boolean(getValue());
      return (
        <span
          className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${
            isActive ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'
          }`}
        >
          {getStatusLabel(isActive, t)}
        </span>
      );
    },
  },
  {
    accessorKey: 'createdAt',
    header: t(i18nKeyContainer.customers.table.columns.createdAt),
    cell: ({ getValue }) => new Date(getValue()).toLocaleDateString(),
  },
];

export default function CustomerDataTable() {
  const { t } = useTranslation();
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  const [currentCustomerId, setCurrentCustomerId] = useState(0);
  const [_viewDialogOpen, setViewDialogOpen] = useState(false);
  const [editDialogOpen, setEditDialogOpen] = useState(false);
  const { showSuccess, showError } = useToast();
  const queryClient = useQueryClient();

  const fetchCustomers = async ({
    page,
    pageSize,
    search,
    sortOrder,
    sortColumn,
  }) => {
    const response = await getCustomers({
      page: page,
      pageSize: pageSize,
      sortColumn: sortColumn,
      sortOrder: sortOrder,
      search: search,
    });
    return response.data;
  };
  const handleView = row => {
    setCurrentCustomerId(row.id);
    setViewDialogOpen(true);
  };
  const handleEdit = row => {
    setCurrentCustomerId(row.id);
    setEditDialogOpen(true);
  };

  const deleteMutation = useMutation({
    mutationFn: deleteCustomerById,
    onSuccess: async response => {
      if (!response.success) {
        showError(
          t(i18nKeyContainer.customers.table.toasts.deleteFailedTitle),
          t(i18nKeyContainer.customers.table.toasts.deleteFailedMessage, {
            error: response.error,
          })
        );
        setDeleteDialogOpen(false);
        return;
      }

      showSuccess(
        t(i18nKeyContainer.customers.table.toasts.deleteSuccessTitle),
        t(i18nKeyContainer.customers.table.toasts.deleteSuccessMessage)
      );
      setDeleteDialogOpen(false);
      await queryClient.invalidateQueries({ queryKey: queryKeys.customers.all });
    },
  });

  const tableProps = useServerSideDataTable(fetchCustomers, {
    queryKey: queryKeys.customers.table('catalog'),
  });

  const handleDeleteConfirm = async () => {
    deleteMutation.mutate(currentCustomerId);
  };

  return (
    <>
      <DataTable
        data={tableProps.data}
        columns={getDefaultColumns(t)}
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
          setCurrentCustomerId(row.id);
          setDeleteDialogOpen(true);
        }}
      />
      {editDialogOpen && (
        <AddUpdateCustomer
          isOpen={editDialogOpen}
          onClose={() => setEditDialogOpen(false)}
          onSuccess={() => {
            queryClient.invalidateQueries({ queryKey: queryKeys.customers.all });
          }}
          customerId={currentCustomerId}
        />
      )}
      {deleteDialogOpen && (
        <ConfirmationDialog
          isOpen={deleteDialogOpen}
          title={t(i18nKeyContainer.customers.table.dialogs.deleteTitle)}
          message={t(i18nKeyContainer.customers.table.dialogs.deleteMessage)}
          onCancel={() => setDeleteDialogOpen(false)}
          onConfirm={handleDeleteConfirm}
        />
      )}
    </>
  );
}
