import DataTable from '@components/DataTable/DataTable';
import { getAllProducts } from '@services/products/productService';
import useServerSideDataTable from '../../../hooks/useServerSideDataTable';
export default function ProductDataTable() {
  const fetchProducts = async ({ page, pageSize }) => {
    const response = await getAllProducts({ page: page, pageSize: pageSize });
    return response;
  };

  const tableProps = useServerSideDataTable(fetchProducts);

  return (
    <DataTable
      data={tableProps.data}
      columns={defaultColumns}
      totalRows={tableProps.totalRows}
      pageIndex={tableProps.pageIndex}
      pageSize={tableProps.pageSize}
      onPageChange={tableProps.onPageChange}
      onPageSizeChange={tableProps.onPageSizeChange}
      //   onSortingChange={tableProps.onSortingChange}
      //   onFilterChange={tableProps.onFilterChange}
      loading={tableProps.loading}
    />
  );
}

const defaultColumns = [
  {
    accessorKey: 'sku',
    header: 'SKU',
  },
  {
    accessorKey: 'name',
    header: 'Product',
    cell: ({ row }) => (
      <div>
        <div className='font-medium'>{row.original.name}</div>
        <div className='text-sm text-gray-500'>{row.original.categoryName}</div>
      </div>
    ),
  },
  {
    accessorKey: 'unitPrice',
    header: 'Price',
    cell: ({ getValue }) => `$${getValue().toFixed(2)}`,
  },
  {
    accessorKey: 'costPrice',
    header: 'Cost',
    cell: ({ getValue }) => `$${getValue().toFixed(2)}`,
  },

  {
    accessorKey: 'isActive',
    header: 'Status',
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
  {
    id: 'actions',
    header: 'Actions',
    cell: () => (
      <button className='text-gray-400 hover:text-gray-600'>•••</button>
    ),
  },
];
