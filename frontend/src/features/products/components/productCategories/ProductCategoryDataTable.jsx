import { useMemo, useState } from 'react';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import SimpleDataTable from '@components/DataTable/SimpleDataTable';
import {
  deleteProductCategory,
  getAllProductCategories,
} from '@features/products/services/productCategoryApi';
import ProductCategoryView from './ProductCategoryView';
import AddProductCategory from './AddProductCategory';
import ConfirmationDialog from '@components/ui/ConfirmationDialog';
import { useToast } from '@shared/context/ToastContext';
import { useTranslation } from 'react-i18next';

import i18nKeyContainer from '@shared/lib/i18n/keyContainer';
import { queryKeys } from '@shared/lib/queryKeys';
import { formatAppDate } from '@shared/utils/dateFormatter';

export default function ProductCategoryDataTable() {
  const { t } = useTranslation();
  const [viewDialogOpen, setViewDialogOpen] = useState(false);
  const [addEditDialogOpen, setAddEditDialogOpen] = useState(false);
  const [currentProductCategoryId, setCurrentProductCategoryId] = useState(0);
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  const { showSuccess, showError } = useToast();
  const queryClient = useQueryClient();
  const columns = useMemo(() => getDefaultColumns(t), [t]);

  const { data = [], isLoading } = useQuery({
    queryKey: queryKeys.products.categories(),
    queryFn: getAllProductCategories,
  });

  const deleteMutation = useMutation({
    mutationFn: deleteProductCategory,
    onSuccess: async result => {
      setDeleteDialogOpen(false);
      if (!result.success) {
        showError(
          t(i18nKeyContainer.products.categories.table.toasts.deleteError, {
            error: result.message,
          })
        );
        return;
      }
      showSuccess(t(i18nKeyContainer.products.categories.table.toasts.deleteSuccess));
      await queryClient.invalidateQueries({ queryKey: queryKeys.products.categories() });
      await queryClient.invalidateQueries({ queryKey: queryKeys.products.summary() });
    },
  });

  const handleView = row => {
    setCurrentProductCategoryId(row.id);
    setViewDialogOpen(true);
  };

  const handleEdit = row => {
    setCurrentProductCategoryId(row.id);
    setAddEditDialogOpen(true);
  };

  const handleDelete = async () => {
    deleteMutation.mutate(currentProductCategoryId);
  };

  const handleAddEditSuccess = async () => {
    setAddEditDialogOpen(false);
    await queryClient.invalidateQueries({ queryKey: queryKeys.products.categories() });
    await queryClient.invalidateQueries({ queryKey: queryKeys.products.summary() });
  };

  const handleAddEditClose = () => {
    setAddEditDialogOpen(false);
    setCurrentProductCategoryId(0);
  };

  return (
    <>
      <SimpleDataTable
        data={data}
        loading={isLoading}
        columns={columns}
        onView={handleView}
        onEdit={handleEdit}
        onDelete={row => {
          setCurrentProductCategoryId(row.id);
          setDeleteDialogOpen(true);
        }}
      />
      {viewDialogOpen && (
        <ProductCategoryView
          open={viewDialogOpen}
          onOpenChange={setViewDialogOpen}
          categoryId={currentProductCategoryId}
        />
      )}

      {addEditDialogOpen && (
        <AddProductCategory
          isOpen={addEditDialogOpen}
          onClose={handleAddEditClose}
          categoryId={currentProductCategoryId}
          onSuccess={handleAddEditSuccess}
        />
      )}
      {deleteDialogOpen && (
        <ConfirmationDialog
          isOpen={deleteDialogOpen}
          onClose={() => setDeleteDialogOpen(false)}
          onConfirm={handleDelete}
          type='delete'
          title={t(i18nKeyContainer.products.categories.table.dialogs.deleteTitle)}
          message={t(i18nKeyContainer.products.categories.table.dialogs.deleteMessage)}
        />
      )}
    </>
  );
}
const getDefaultColumns = t => [
  {
    accessorKey: 'id',
    header: t(i18nKeyContainer.products.categories.table.columns.id),
  },
  {
    accessorKey: 'name',
    header: t(i18nKeyContainer.products.categories.table.columns.name),
  },
  {
    accessorKey: 'parentName',
    header: t(i18nKeyContainer.products.categories.table.columns.parentCategory),
    cell: ({ getValue }) =>
      getValue() ||
      t(i18nKeyContainer.products.categories.table.columns.mainCategory),
  },
  {
    accessorKey: 'createdAt',
    header: t(i18nKeyContainer.products.categories.table.columns.createdAt),
    cell: ({ getValue }) => formatAppDate(getValue()),
  },
  {
    accessorKey: 'createdByUserName',
    header: t(i18nKeyContainer.products.categories.table.columns.createdBy),
  },
];
