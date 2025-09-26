import DataTable from './DataTable';

// Example usage of the generic DataTable component

const ExampleUsage = () => {
  // Sample data - replace with your actual data
  const sampleData = [
    {
      sku: 'IPH15P-128-BLK',
      product: 'iPhone 15 Pro',
      category: 'Electronics • Apple',
      price: 999.0,
      stock: 45,
      location: 'A1-B2-C3',
      status: 'active',
    },
    {
      sku: 'MBA-M2-256-SLV',
      product: 'MacBook Air M2',
      category: 'Computers • Apple',
      price: 1299.0,
      stock: 8,
      location: 'A2-B1-C2',
      status: 'active',
    },
    {
      sku: 'APP-2ND-GEN',
      product: 'AirPods Pro (2nd Gen)',
      category: 'Audio • Apple',
      price: 249.0,
      stock: 2,
      location: 'B1-C2-D1',
      status: 'active',
    },
  ];

  // Column definitions - customize based on your data structure
  const columns = [
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
      accessorKey: 'price',
      header: 'Price',
      cell: ({ getValue }) => `$${getValue().toFixed(2)}`,
    },
    {
      accessorKey: 'stock',
      header: 'Stock',
      cell: ({ getValue }) => {
        const stock = getValue();
        return (
          <span className={stock <= 5 ? 'text-red-600 font-medium' : ''}>
            {stock}
            {stock <= 5 && ' ⚠️'}
          </span>
        );
      },
    },
    {
      accessorKey: 'location',
      header: 'Location',
    },
    {
      accessorKey: 'status',
      header: 'Status',
      cell: ({ getValue }) => (
        <span className='inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-green-100 text-green-800'>
          ✓ {getValue()}
        </span>
      ),
    },
    {
      id: 'actions',
      header: 'Actions',
      cell: () => (
        <button className='text-gray-400 hover:text-gray-600'>•••</button>
      ),
    },
  ];

  return (
    <div className='p-6'>
      <div className='mb-6'>
        <h1 className='text-2xl font-semibold text-gray-900'>
          Product Catalog
        </h1>
      </div>

      <DataTable
        data={sampleData}
        columns={columns}
        searchPlaceholder='Search products...'
        pageSize={10}
        enableSorting={true}
        enableFiltering={true}
        enablePagination={true}
      />
    </div>
  );
};

export default ExampleUsage;

/*
HOW TO USE THE GENERIC DATATABLE:

1. Import the DataTable component:
   import DataTable from '../components/DataTable';

2. Prepare your data array:
   const data = [{ id: 1, name: 'Item 1' }, ...];

3. Define your columns:
   const columns = [
     {
       accessorKey: 'id', // The key from your data object
       header: 'ID',      // The header text to display
     },
     {
       accessorKey: 'name',
       header: 'Name',
       cell: ({ getValue }) => {
         // Custom cell rendering (optional)
         return <strong>{getValue()}</strong>;
       },
     },
   ];

4. Use the DataTable:
   <DataTable 
     data={data}
     columns={columns}
     searchPlaceholder="Search items..."
     pageSize={10}
     enableSorting={true}
     enableFiltering={true}
     enablePagination={true}
   />

PROPS:
- data: Array of objects to display
- columns: Array of column definitions
- pageSize: Number of rows per page (default: 10)
- enableSorting: Enable column sorting (default: true)
- enableFiltering: Enable global search (default: true)
- enablePagination: Enable pagination (default: true)
- searchPlaceholder: Placeholder text for search input
- className: Additional CSS classes for the table container
*/
