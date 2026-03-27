import React from 'react';
import DataTable from './DataTable';
import useServerSideDataTable from '../../hooks/useServerSideDataTable';

// Example service function that matches your backend API
const fetchExampleData = async ({
  Page,
  PageSize,
  SortColumn,
  SortOrder,
  search,
}) => {
  // This should call your actual API endpoint
  // Example: return await api.get('/your-endpoint', { params: { Page, PageSize, SortColumn, SortOrder, search } });

  // Mock response for demonstration
  console.log('API call with params:', {
    Page,
    PageSize,
    SortColumn,
    SortOrder,
    search,
  });

  // Simulate API delay
  await new Promise(resolve => setTimeout(resolve, 500));

  // Mock data response
  return {
    item: [
      { id: 1, name: 'Product 1', price: 100, category: 'Electronics' },
      { id: 2, name: 'Product 2', price: 200, category: 'Clothing' },
      // ... more mock data
    ],
    totalCount: 100, // Total number of records
  };
};

const ExampleServerSideDataTable = () => {
  // Use the server-side data table hook
  const tableProps = useServerSideDataTable(fetchExampleData, {
    initialPageSize: 10,
    onError: error => {
      console.error('Data fetch error:', error);
      // Handle error (show notification, etc.)
    },
  });

  // Define your table columns
  const columns = [
    {
      accessorKey: 'id',
      header: 'ID',
      enableSorting: true,
    },
    {
      accessorKey: 'name',
      header: 'Name',
      enableSorting: true,
    },
    {
      accessorKey: 'price',
      header: 'Price',
      enableSorting: true,
      cell: ({ getValue }) => `$${getValue().toFixed(2)}`,
    },
    {
      accessorKey: 'category',
      header: 'Category',
      enableSorting: true,
    },
  ];

  return (
    <div className='p-6'>
      <h2 className='text-2xl font-bold mb-4'>Server-Side DataTable Example</h2>
      <p className='text-gray-600 mb-6'>
        This DataTable supports server-side pagination, sorting, and filtering.
        When you sort or filter, it automatically resets to the first page.
      </p>

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
        enableSorting={true}
        enableFiltering={true}
        enablePagination={true}
        searchPlaceholder='Search products...'
        searchDebounceMs={500}
        pageSizes={[5, 10, 20, 50]}
      />

      {tableProps.error && (
        <div className='mt-4 p-4 bg-red-100 border border-red-400 text-red-700 rounded'>
          Error loading data: {tableProps.error.message}
        </div>
      )}
    </div>
  );
};

export default ExampleServerSideDataTable;
