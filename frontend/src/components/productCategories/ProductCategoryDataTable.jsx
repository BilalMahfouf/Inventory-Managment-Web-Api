import { useState, useEffect } from 'react';
import SimpleDataTable from '../DataTable/SimpleDataTable';
import {
  deleteProductCategory,
  getAllProductCategories,
} from '@/services/products/productCategoryService';
import ProductCategoryView from './ProductCategoryView';
import AddProductCategory from './AddProductCategory';
import ConfirmationDialog from '../ui/ConfirmationDialog';
import { useToast } from '@/context/ToastContext';

export default function ProductCategoryDataTable({ refresh }) {
  const [viewDialogOpen, setViewDialogOpen] = useState(false);
  const [addEditDialogOpen, setAddEditDialogOpen] = useState(false);
  const [currentProductCategoryId, setCurrentProductCategoryId] = useState(0);
  const [data, setData] = useState(null);
  const [isLoading, setIsLoading] = useState(false);
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  const { showSuccess, showError } = useToast();

  const fetchProductCategories = async () => {
    setIsLoading(true);
    const responseData = await getAllProductCategories();
    setData(responseData);
    setIsLoading(false);
  };

  useEffect(() => {
    fetchProductCategories();
  }, [refresh]);

  const handleView = row => {
    setCurrentProductCategoryId(row.id);
    setViewDialogOpen(true);
  };

  const handleEdit = row => {
    setCurrentProductCategoryId(row.id);
    setAddEditDialogOpen(true);
  };

  const handleDelete = async () => {
    console.log('delete is clicked');

    const result = await deleteProductCategory(currentProductCategoryId);
    setDeleteDialogOpen(false);
    if (!result.success) {
      showError(`failed to delete product category: ${result.message}`);
      return;
    }
    showSuccess('Product Category deleted successfully');
    await fetchProductCategories();
  };

  const handleAddEditSuccess = () => {
    setAddEditDialogOpen(false);
    fetchProductCategories();
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
        columns={defaultColumns}
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
          title='Delete Product Category'
          message='Are you sure you want to delete this Category, this action cannot be undone.'
        />
      )}
    </>
  );
}
const defaultColumns = [
  {
    accessorKey: 'id',
    header: 'ID',
  },
  {
    accessorKey: 'name',
    header: 'Name',
  },
  {
    accessorKey: 'parentName',
    header: 'Parent Category',
    cell: ({ getValue }) => getValue() || 'Main Category',
  },
  {
    accessorKey: 'createdAt',
    header: 'Created At',
    cell: ({ getValue }) => new Date(getValue()).toLocaleDateString(),
  },
  {
    accessorKey: 'createdByUserName',
    header: 'Created By',
  },
];
