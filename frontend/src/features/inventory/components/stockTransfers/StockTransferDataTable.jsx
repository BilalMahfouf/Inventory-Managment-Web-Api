import { useState } from 'react';
import DataTable from '@components/DataTable/DataTable';
import { getAllStockTransfers } from '@features/inventory/services/stockTransferApi';
import useServerSideDataTable from '@shared/hooks/useServerSideDataTable';
import ViewStockTransfer from './view/ViewStockTransfer';
import { useTranslation } from 'react-i18next';
import i18nKeyContainer from '@shared/lib/i18n/keyContainer';
import { queryKeys } from '@shared/lib/queryKeys';
import { formatAppDate } from '@shared/utils/dateFormatter';

const getLocalizedTransferStatus = (status, t) => {
  if (status === 'Completed') {
    return t(i18nKeyContainer.inventory.shared.status.completed);
  }

  if (status === 'Pending') {
    return t(i18nKeyContainer.inventory.shared.status.pending);
  }

  if (status === 'InTransit' || status === 'In Transit') {
    return t(i18nKeyContainer.inventory.shared.status.inTransit);
  }

  if (status === 'Cancelled') {
    return t(i18nKeyContainer.inventory.shared.status.cancelled);
  }

  return status;
};

const getDefaultColumns = t => [
  {
    accessorKey: 'product',
    header: t(i18nKeyContainer.inventory.stockTransfers.table.columns.product),
  },
  {
    accessorKey: 'fromLocation',
    header: t(i18nKeyContainer.inventory.stockTransfers.table.columns.fromLocation),
  },
  {
    accessorKey: 'toLocation',
    header: t(i18nKeyContainer.inventory.stockTransfers.table.columns.toLocation),
  },
  {
    accessorKey: 'quantity',
    header: t(i18nKeyContainer.inventory.stockTransfers.table.columns.quantity),
    cell: ({ getValue }) => getValue().toFixed(2),
  },
  {
    accessorKey: 'status',
    header: t(i18nKeyContainer.inventory.stockTransfers.table.columns.status),
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
          {getLocalizedTransferStatus(status, t)}
        </span>
      );
    },
  },
  {
    accessorKey: 'userName',
    header: t(i18nKeyContainer.inventory.stockTransfers.table.columns.user),
  },
  {
    accessorKey: 'createdAt',
    header: t(i18nKeyContainer.inventory.stockTransfers.table.columns.createdAt),
    cell: ({ getValue }) => formatAppDate(getValue()),
  },
];

export default function StockTransferDataTable() {
  const { t } = useTranslation();
  const [viewDialogOpen, setViewDialogOpen] = useState(false);
  const [selectedTransferId, setSelectedTransferId] = useState(null);

  const handleView = row => {
    console.log('Viewing stock transfer with ID:', row.id);
    setSelectedTransferId(row.id);
    setViewDialogOpen(true);
  };

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
    return { item: [], totalCount: 0 };
  };

  const tableProps = useServerSideDataTable(fetchStockTransfers, {
    queryKey: queryKeys.inventory.stockTransfers('list'),
  });

  return (
    <>
      <DataTable
        data={tableProps.data}
        columns={getDefaultColumns(t)}
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

      <ViewStockTransfer
        open={viewDialogOpen}
        onOpenChange={setViewDialogOpen}
        transferId={selectedTransferId}
      />
    </>
  );
}
