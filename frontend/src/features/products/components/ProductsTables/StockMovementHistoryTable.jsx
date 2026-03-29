import { getStockMovementsHistory } from '@features/products/services/productApi';
import useServerSideDataTable from '@shared/hooks/useServerSideDataTable';
import DataTable from '@components/DataTable/DataTable';
import { useMemo } from 'react';
import { useTranslation } from 'react-i18next';

import i18nKeyContainer from '@shared/lib/i18n/keyContainer';
import { queryKeys } from '@shared/lib/queryKeys';
import { formatAppDate } from '@shared/utils/dateFormatter';

export default function StockMovementHistoryTable() {
  const { t } = useTranslation();

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

  const tableProps = useServerSideDataTable(fetchData, {
    queryKey: queryKeys.products.stockMovements('history'),
  });
  const columns = useMemo(() => getDefaultColumns(t), [t]);

  return (
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
    />
  );
}
// Component for rendering type badges
const TypeBadge = ({ type, t }) => {
  const getTypeConfig = type => {
    const typeStr = type?.toLowerCase();

    switch (typeStr) {
      case 'in':
        return {
          label: t(i18nKeyContainer.products.stockMovements.badges.in),
          bgColor: 'bg-green-100',
          textColor: 'text-green-700',
          borderColor: 'border-green-200',
        };
      case 'out':
        return {
          label: t(i18nKeyContainer.products.stockMovements.badges.out),
          bgColor: 'bg-red-100',
          textColor: 'text-red-700',
          borderColor: 'border-red-200',
        };
      case 'adjustment':
      case 'transfer':
        return {
          label: t(i18nKeyContainer.products.stockMovements.badges.transfer),
          bgColor: 'bg-blue-100',
          textColor: 'text-blue-700',
          borderColor: 'border-blue-200',
        };
      default:
        return {
          label:
            type || t(i18nKeyContainer.products.stockMovements.badges.unknown),
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
const getDefaultColumns = t => [
  {
    accessorKey: 'createdAt',
    header: t(i18nKeyContainer.products.stockMovements.columns.date),
    cell: ({ row }) => formatAppDate(row.original.createdAt),
  },
  {
    accessorKey: 'product',
    header: t(i18nKeyContainer.products.stockMovements.columns.product),
    cell: ({ row }) => (
      <div>
        <div className='font-medium'>{row.original.product}</div>
        <div className='text-sm text-gray-500'>{row.original.sku}</div>
      </div>
    ),
  },
  {
    accessorKey: 'type',
    header: t(i18nKeyContainer.products.stockMovements.columns.type),
    cell: ({ getValue }) => <TypeBadge type={getValue()} t={t} />,
  },
  {
    accessorKey: 'quantity',
    header: t(i18nKeyContainer.products.stockMovements.columns.quantity),
    cell: ({ getValue }) => `${getValue().toFixed(2)}`,
  },
  {
    accessorKey: 'reason',
    header: t(i18nKeyContainer.products.stockMovements.columns.reason),
  },
  {
    accessorKey: 'createdByUser',
    header: t(i18nKeyContainer.products.stockMovements.columns.processedBy),
  },
];
