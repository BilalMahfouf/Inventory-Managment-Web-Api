import { useToast } from '@/context/ToastContext';
import useServerSideDataTable from '@/hooks/useServerSideDataTable';
import { useState } from 'react';
import {
  deleteCustomerById,
  getCustomers,
} from '@/services/customers/customerService';
import DataTable from '../DataTable/DataTable';
import AddUpdateCustomer from './AddUpdateCustomer';
import ConfirmationDialog from '../ui/ConfirmationDialog';

export default function CustomerDataTable() {
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  const [currentCustomerId, setCurrentCustomerId] = useState(0);
  const [viewDialogOpen, setViewDialogOpen] = useState(false);
  const [editDialogOpen, setEditDialogOpen] = useState(false);
  const { showSuccess, showError } = useToast();

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

  const handleDeleteConfirm = async () => {
    const response = await deleteCustomerById(currentCustomerId);
    if (!response.success) {
      showError('Failed to delete customer. ' + response.error);
      setDeleteDialogOpen(false);
      return;
    }
    showSuccess('Customer deleted successfully.');
    setDeleteDialogOpen(false);
    tableProps.refresh();
  };

  const tableProps = useServerSideDataTable(fetchCustomers);

  return (
    <>
      <DataTable
        data={tableProps.data}
        columns={defaultColumns}
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
            tableProps.refresh();
          }}
          customerId={currentCustomerId}
        />
      )}
      {deleteDialogOpen && (
        <ConfirmationDialog
          isOpen={deleteDialogOpen}
          title='Delete Customer'
          message='Are you sure you want to delete this customer? This action cannot be undone.'
          onCancel={() => setDeleteDialogOpen(false)}
          onConfirm={handleDeleteConfirm}
        />
      )}
    </>
  );
}

const defaultColumns = [
  {
    accessorKey: 'name',
    header: 'Customer',
    cell: ({ row }) => (
      <div>
        <div className='font-medium'>{row.original.name}</div>
        <div className='text-sm text-gray-500'>{row.original.email}</div>
      </div>
    ),
  },
  {
    accessorKey: 'phone',
    header: 'Phone',
  },
  {
    accessorKey: 'customerCategoryName',
    header: 'Category',
  },
  {
    accessorKey: 'totalOrders',
    header: 'Total Orders',
    cell: ({ getValue }) => `${getValue().toFixed(2)}`,
  },

  {
    accessorKey: 'totalSpent',
    header: 'Total Spent',
    cell: ({ getValue }) => `$${getValue().toFixed(2)}`,
  },
  {
    accessorKey: 'isActive',
    header: 'IsActive',
    cell: ({ getValue }) => (
      <span className='inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-green-100 text-green-800'>
        ✓ {getValue()}
      </span>
    ),
  },
  {
    accessorKey: 'createdAt',
    header: 'Created At',
    cell: ({ getValue }) => new Date(getValue()).toLocaleDateString(),
  },
];
