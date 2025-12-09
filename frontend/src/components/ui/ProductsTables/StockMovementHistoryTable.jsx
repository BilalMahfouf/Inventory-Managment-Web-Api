import { getStockMovementsHistory } from '@services/products/productService';
import useServerSideDataTable from '@hooks/useServerSideDataTable';
import DataTable from '@components/DataTable/DataTable';

export default function StockMovementHistoryTable() {
  const fetchData = async ({
    page,
    pageSize,
    sortColumn,
    sortOrder,
    search,
  }) => {
    const response = await getStockMovementsHistory({
      page,
      pageSize,
      sortColumn,
      sortOrder,
      search,
    });
    return response;
  };

  const tableProps = useServerSideDataTable(fetchData);

  return (
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
    />
  );
}
// Component for rendering type badges
const TypeBadge = ({ type }) => {
  const getTypeConfig = type => {
    const typeStr = type?.toLowerCase();

    switch (typeStr) {
      case 'in':
        return {
          label: '↑ In',
          bgColor: 'bg-green-100',
          textColor: 'text-green-700',
          borderColor: 'border-green-200',
        };
      case 'out':
        return {
          label: '↓ Out',
          bgColor: 'bg-red-100',
          textColor: 'text-red-700',
          borderColor: 'border-red-200',
        };
      case 'adjustment':
      case 'transfer':
        return {
          label: '⟲ Transfer',
          bgColor: 'bg-blue-100',
          textColor: 'text-blue-700',
          borderColor: 'border-blue-200',
        };
      default:
        return {
          label: type || 'unknown',
          bgColor: 'bg-gray-100',
          textColor: 'text-gray-700',
          borderColor: 'border-gray-200',
        };
    }
  };

  const config = getTypeConfig(type);

  return (
    <span
      className={`
        inline-flex items-center px-2.5 py-1 
        rounded-full text-xs font-medium
        border ${config.bgColor} ${config.textColor} ${config.borderColor}
      `}
    >
      {config.label}
    </span>
  );
};
const defaultColumns = [
  {
    accessorKey: 'createdAt',
    header: 'Date',
    cell: ({ row }) => new Date(row.original.createdAt).toLocaleDateString(),
  },
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
    accessorKey: 'type',
    header: 'Type',
    cell: ({ getValue }) => <TypeBadge type={getValue()} />,
  },
  {
    accessorKey: 'quantity',
    header: 'Quantity',
    cell: ({ getValue }) => `${getValue().toFixed(2)}`,
  },
  {
    accessorKey: 'reason',
    header: 'Reason',
  },
  {
    accessorKey: 'createdByUser',
    header: 'Processed By',
  },
];
