import { useState } from 'react';
import DataTable from '@components/DataTable/DataTable';
import { getAllProducts } from '@services/products/productService';
import useServerSideDataTable from '../../../hooks/useServerSideDataTable';
import ProductViewDialog from './ProductViewDialog';
import { AddProduct } from '@/components/products';
import ConfirmationDialog from '../ConfirmationDialog';
import { deleteProduct } from '@/services/products/productService';
import { useToast } from '@/context/ToastContext';
import { ShowerHead } from 'lucide-react';
export default function ProductDataTable() {
  const [viewDialogOpen, setViewDialogOpen] = useState(false);
  const [selectedProductId, setSelectedProductId] = useState(0);
  const [updateDialogOpen, setUpdateDialogOpen] = useState(false);
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  const [currentProductId, setCurrentProductId] = useState(0);
  const { showError, showSuccess } = useToast();

  const fetchProducts = async ({
    page,
    pageSize,
    sortColumn,
    sortOrder,
    search,
  }) => {
    const response = await getAllProducts({
      page: page,
      pageSize: pageSize,
      sortColumn: sortColumn,
      sortOrder: sortOrder,
      search: search,
    });
    return response;
  };

  const tableProps = useServerSideDataTable(fetchProducts);

  const handleView = row => {
    console.log('view is clicked', row.id);
    setSelectedProductId(row.id);
    setViewDialogOpen(true);
  };
  const handleEdit = row => {
    setSelectedProductId(row.id);
    setUpdateDialogOpen(true);
  };
  const handleEditClose = () => {
    setUpdateDialogOpen(false);
    tableProps.refresh();
  };
  const handleDelete = () => {
    console.log('delete is clicked', currentProductId);
    deleteProduct(currentProductId);

    showSuccess(
      `Product is Deleted `,
      `Product with id ${currentProductId} is deleted successfully`
    );
    setDeleteDialogOpen(false);
  };

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
          setCurrentProductId(row.id);
          setDeleteDialogOpen(true);
        }}
      />
      {viewDialogOpen && (
        <ProductViewDialog
          open={viewDialogOpen}
          onOpenChange={setViewDialogOpen}
          productId={selectedProductId}
        />
      )}
      {updateDialogOpen && (
        <AddProduct
          isOpen={updateDialogOpen}
          onClose={handleEditClose}
          onSubmit={console.log('sumbited ')}
          productId={selectedProductId}
          isLoading={true}
        />
      )}
      {deleteDialogOpen && (
        <ConfirmationDialog
          isOpen={deleteDialogOpen}
          onConfirm={handleDelete}
          onClose={() => {
            setDeleteDialogOpen(false);
            showError('Delete cancelled', 'Product delete action is cancelled');
          }}
          title='Delete Product'
          message='Are you sure you want to delete this product? This action cannot be undone.'
        />
      )}
    </>
  );
}

const defaultColumns = [
  {
    accessorKey: 'sku',
    header: 'SKU',
  },
  {
    accessorKey: 'product',
    header: 'Product',
    cell: ({ row }) => (
      <div>
        <div className='font-medium'>{row.original.product}</div>
        <div className='text-sm text-gray-500'>{row.original.category}</div>
      </div>
    ),
  },
  {
    accessorKey: 'stock',
    header: 'Stock',
  },
  {
    accessorKey: 'price',
    header: 'Price',
    cell: ({ getValue }) => `$${getValue().toFixed(2)}`,
  },
  {
    accessorKey: 'cost',
    header: 'Cost',
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
