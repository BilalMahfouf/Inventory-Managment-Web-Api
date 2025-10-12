import { useState, useEffect } from 'react';
import SimpleDataTable from '../DataTable/SimpleDataTable';
import { getAllProductCategories } from '@/services/products/productCategoryService';
import ProductCategoryView from './ProductCategoryView';

export default function ProductCategoryDataTable() {
  const [viewDialogOpen, setViewDialogOpen] = useState(false);
  const [currentProductCategoryId, setCurrentProductCategoryId] = useState(0);
  const [data, setData] = useState(null);
  const [isLoading, setIsLoading] = useState(false);

  useEffect(() => {
    const fetchProductCategories = async () => {
      setIsLoading(true);
      const responseData = await getAllProductCategories();
      setData(responseData);
      setIsLoading(false);
    };

    fetchProductCategories();
  }, []);

  const handleView = row => {
    setCurrentProductCategoryId(row.id);
    setViewDialogOpen(true);
  };
  const handleEdit = () => {
    console.log('edit is clicked');
  };
  const handleDelete = () => {
    console.log('delete is clicked');
  };

  return (
    <>
      <SimpleDataTable
        data={data}
        loading={isLoading}
        columns={defaultColumns}
        onView={handleView}
        onEdit={handleEdit}
        onDelete={handleDelete}
      />
      <ProductCategoryView
        open={viewDialogOpen}
        onOpenChange={setViewDialogOpen}
        categoryId={currentProductCategoryId}
      />
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
