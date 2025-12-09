import { useState } from 'react';
import DataTable from '../DataTable/DataTable';
import useServerSideDataTable from '@/hooks/useServerSideDataTable';
import {
  getAllInventory,
  deleteInventoryById,
} from '@/services/inventoryService';
import { CheckCircle, AlertTriangle, XCircle } from 'lucide-react';
import AddUpdateInventory from './AddUpdateInventory';
import ViewInventoryDialog from './ViewInventoryDialog';
import ConfirmationDialog from '../ui/ConfirmationDialog';
import { useToast } from '@/context/ToastContext';
export default function InventoryDataTable() {
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  const [currentInventoryId, setCurrentInventoryId] = useState(0);
  const [viewDialogOpen, setViewDialogOpen] = useState(false);
  const [editDialogOpen, setEditDialogOpen] = useState(false);
  const { showSuccess, showError } = useToast();

  const handleDelete = async () => {
    const response = await deleteInventoryById(currentInventoryId);
    if (response.success) {
      setDeleteDialogOpen(false);
      showSuccess(
        'Success',
        `Inventory with Id ${currentInventoryId} deleted successfully.`
      );
      return;
    }
    showError('Error', `Error: ${response.error}.`);
    setDeleteDialogOpen(false);
    return;
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

  const tableProps = useServerSideDataTable(fetchInventories);

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
        />
      )}
      {deleteDialogOpen && (
        <ConfirmationDialog
          isOpen={deleteDialogOpen}
          onClose={() => setDeleteDialogOpen(false)}
          title='Delete Inventory Item'
          message='Are you sure you want to delete this inventory item? This action cannot be undone.'
          confirmText='Delete Inventory'
          onConfirm={handleDelete}
        />
      )}
    </>
  );
}

const defaultColumns = [
  {
    accessorKey: 'product',
    header: 'Product',
    cell: ({ row }) => (
      <div>
        <div className='font-medium'>{row.original.product}</div>
        <div className='text-sm text-gray-500'>{row.original.sku}</div>
      </div>
    ),
  },
  {
    accessorKey: 'location',
    header: 'Location',
  },
  {
    accessorKey: 'quantity',
    header: 'Quantity',
    cell: ({ getValue }) => `${getValue().toFixed(2)}`,
  },
  {
    accessorKey: 'reorder',
    header: 'Reorder',
    cell: ({ getValue }) => `${getValue().toFixed(2)}`,
  },

  {
    accessorKey: 'max',
    header: 'Max',
  },
  {
    accessorKey: 'status',
    header: 'Status',
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
          {status}
        </span>
      );
    },
  },
  {
    accessorKey: 'potentialProfit',
    header: 'Potential Profit',
    cell: ({ getValue }) => `$${getValue().toFixed(2)}`,
  },
];
