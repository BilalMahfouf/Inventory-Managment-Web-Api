import DataTable from '../DataTable/DataTable';
import { getAllStockTransfers } from '@services/stock/stockTransferService';
import useServerSideDataTable from '@/hooks/useServerSideDataTable';

export default function StockTransferDataTable() {
  const handleView = () => {};

  const fetchStockTransfers = async ({
    page,
    pageSize,
    search,
    sortOrder,
    sortColumn,
  }) => {
    const response = await getAllStockTransfers({
      page: page,
      pageSize: pageSize,
      sortColumn: sortColumn,
      sortOrder: sortOrder,
      search: search,
    });
    if (response.success) {
      return response.data;
    }
    return { data: [], totalRows: 0 };
  };

  const tableProps = useServerSideDataTable(fetchStockTransfers);

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
      />
    </>
  );
}

const defaultColumns = [
  {
    accessorKey: 'product',
    header: 'Product',
  },
  {
    accessorKey: 'fromLocation',
    header: 'From Location',
  },
  {
    accessorKey: 'toLocation',
    header: 'To Location',
  },
  {
    accessorKey: 'quantity',
    header: 'Quantity',
    cell: ({ getValue }) => getValue().toFixed(2),
  },
  {
    accessorKey: 'status',
    header: 'Status',
    cell: ({ getValue }) => {
      const status = getValue();

      return (
        <span
          className={`inline-flex items-center gap-1 px-3 py-1 rounded-full text-xs font-medium transition-colors ${
            status === 'Completed'
              ? 'bg-green-100 text-green-700 hover:bg-green-700 hover:text-white'
              : status === 'Pending'
                ? 'bg-yellow-100 text-yellow-700 hover:bg-yellow-600 hover:text-white'
                : 'bg-blue-100 text-blue-700 hover:bg-blue-700 hover:text-white'
          }`}
        >
          {status}
        </span>
      );
    },
  },
  {
    accessorKey: 'userName',
    header: 'User',
  },
  {
    accessorKey: 'createdAt',
    header: 'Created At',
    cell: ({ getValue }) => new Date(getValue()).toLocaleDateString(),
  },
];
