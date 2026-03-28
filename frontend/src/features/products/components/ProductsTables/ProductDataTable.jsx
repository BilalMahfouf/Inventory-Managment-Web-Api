import { useMemo, useState } from 'react';
import DataTable from '@components/DataTable/DataTable';
import { getAllProducts } from '@features/products/services/productApi';
import useServerSideDataTable from '@shared/hooks/useServerSideDataTable';
import ProductViewDialog from './ProductViewDialog';
import { AddProduct } from '@features/products/components/products';
import ConfirmationDialog from '@components/ui/ConfirmationDialog';
import { deleteProduct } from '@features/products/services/productApi';
import { useToast } from '@shared/context/ToastContext';
import { useTranslation } from 'react-i18next';
import i18nKeyContainer from '@shared/lib/i18n/keyContainer';
export default function ProductDataTable() {
  const { t } = useTranslation();
  const [viewDialogOpen, setViewDialogOpen] = useState(false);
  const [selectedProductId, setSelectedProductId] = useState(0);
  const [updateDialogOpen, setUpdateDialogOpen] = useState(false);
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  const [currentProductId, setCurrentProductId] = useState(0);
  const { showError, showSuccess } = useToast();

  const columns = useMemo(() => getDefaultColumns(t), [t]);

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
      t(i18nKeyContainer.products.productTable.toasts.deleteSuccessTitle),
      t(i18nKeyContainer.products.productTable.toasts.deleteSuccessMessage, {
        id: currentProductId,
      })
    );
    setDeleteDialogOpen(false);
  };

  return (
    <>
      <DataTable
        data={tableProps.data}
      columns={columns}
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
            showError(
              t(i18nKeyContainer.products.productTable.toasts.deleteCancelledTitle),
              t(
                i18nKeyContainer.products.productTable.toasts
                  .deleteCancelledMessage
              )
            );
          }}
          title={t(i18nKeyContainer.products.productTable.dialogs.deleteTitle)}
          message={t(i18nKeyContainer.products.productTable.dialogs.deleteMessage)}
        />
      )}
    </>
  );
}

const getDefaultColumns = t => [
  {
    accessorKey: 'sku',
    header: t(i18nKeyContainer.products.productTable.columns.sku),
  },
  {
    accessorKey: 'product',
    header: t(i18nKeyContainer.products.productTable.columns.product),
    cell: ({ row }) => (
      <div>
        <div className='font-medium'>{row.original.product}</div>
        <div className='text-sm text-gray-500'>{row.original.category}</div>
      </div>
    ),
  },
  {
    accessorKey: 'stock',
    header: t(i18nKeyContainer.products.productTable.columns.stock),
  },
  {
    accessorKey: 'price',
    header: t(i18nKeyContainer.products.productTable.columns.price),
    cell: ({ getValue }) => `$${getValue().toFixed(2)}`,
  },
  {
    accessorKey: 'cost',
    header: t(i18nKeyContainer.products.productTable.columns.cost),
    cell: ({ getValue }) => `$${getValue().toFixed(2)}`,
  },

  {
    accessorKey: 'isActive',
    header: t(i18nKeyContainer.products.productTable.columns.isActive),
    cell: ({ getValue }) => {
      const value = getValue();
      const normalized =
        typeof value === 'string' ? value.toLowerCase().trim() : value;
      const isActive = normalized === true || normalized === 'active';

      return (
        <span className='inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-green-100 text-green-800'>
          ✓{' '}
          {isActive
            ? t(i18nKeyContainer.products.shared.status.active)
            : t(i18nKeyContainer.products.shared.status.inactive)}
        </span>
      );
    },
  },
  {
    accessorKey: 'createdAt',
    header: t(i18nKeyContainer.products.productTable.columns.createdAt),
    cell: ({ getValue }) => new Date(getValue()).toLocaleDateString(),
  },
];
