import { useToast } from '@/context/ToastContext';
import useServerSideDataTable from '@/hooks/useServerSideDataTable';
import { useState } from 'react';
import { getCustomers } from '@/services/customers/customerService';
import DataTable from '../DataTable/DataTable';

export default function CustomerDataTable() {
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  const [currentInventoryId, setCurrentInventoryId] = useState(0);
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
    setCurrentInventoryId(row.id);
    setViewDialogOpen(true);
  };
  const handleEdit = row => {
    setCurrentInventoryId(row.id);
    setEditDialogOpen(true);
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
          setCurrentInventoryId(row.id);
          setDeleteDialogOpen(true);
        }}
      />
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
