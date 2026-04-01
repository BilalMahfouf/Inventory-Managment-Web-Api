import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { Eye, Edit, ShoppingCart, Filter, X } from 'lucide-react';
import DataTable from '@components/DataTable/DataTable';
import useServerSideDataTable from '@shared/hooks/useServerSideDataTable';
import Button from '@components/Buttons/Button';
import StatusBadge from './StatusBadge';
import PaymentStatusBadge from './PaymentStatusBadge';
import { getOrders } from '@features/sales/services/salesOrderApi';
import { ORDER_STATUS } from '@features/sales/utils/orderConstants';
import { queryKeys } from '@shared/lib/queryKeys';
import i18nKeyContainer from '@shared/lib/i18n/keyContainer';

/**
 * OrdersDataTable Component
 *
 * Server-side paginated table for orders with filters.
 *
 * @param {Object} props - Component props
 * @param {Object} props.initialFilters - Optional default filters
 */
export default function OrdersDataTable({ initialFilters = {} }) {
  const { t } = useTranslation();
  const navigate = useNavigate();
  const [filters, setFilters] = useState({
    status: initialFilters.status || '',
    customerId: initialFilters.customerId || '',
    dateFrom: initialFilters.dateFrom || '',
    dateTo: initialFilters.dateTo || '',
  });
  const [showFilters, setShowFilters] = useState(false);

  const getColumns = () => [
    {
      accessorKey: 'orderNumber',
      header: t(i18nKeyContainer.sales.orders.table.columns.orderNumber),
      cell: ({ row }) => (
        <button
          onClick={() => navigate(`/orders/${row.original.id}`)}
          className="font-medium text-blue-600 hover:text-blue-800 hover:underline"
        >
          #{row.original.orderNumber || row.original.id}
        </button>
      ),
    },
    {
      accessorKey: 'customerName',
      header: t(i18nKeyContainer.sales.orders.table.columns.customer),
      cell: ({ row }) => (
        <span className={row.original.customerId ? '' : 'text-gray-500 italic'}>
          {row.original.customerName || t(i18nKeyContainer.sales.orders.table.walkIn)}
        </span>
      ),
    },
    {
      accessorKey: 'orderDate',
      header: t(i18nKeyContainer.sales.orders.table.columns.date),
      cell: ({ getValue }) => {
        const date = getValue();
        if (!date) return '-';
        return new Date(date).toLocaleDateString();
      },
    },
    {
      accessorKey: 'itemCount',
      header: t(i18nKeyContainer.sales.orders.table.columns.items),
      cell: ({ row }) => row.original.items?.length || row.original.itemCount || 0,
    },
    {
      accessorKey: 'total',
      header: t(i18nKeyContainer.sales.orders.table.columns.total),
      cell: ({ getValue }) => {
        const total = getValue();
        return `$${(total || 0).toFixed(2)}`;
      },
    },
    {
      accessorKey: 'status',
      header: t(i18nKeyContainer.sales.orders.table.columns.status),
      cell: ({ getValue }) => <StatusBadge status={getValue()} />,
    },
    {
      accessorKey: 'paymentStatus',
      header: t(i18nKeyContainer.sales.orders.table.columns.paymentStatus),
      cell: ({ getValue }) => <PaymentStatusBadge status={getValue()} />,
    },
  ];

  const fetchOrders = async ({ page, pageSize, search }) => {
    const response = await getOrders({
      pageNumber: page,
      pageSize: pageSize,
      status: filters.status || undefined,
      customerId: filters.customerId || undefined,
      dateFrom: filters.dateFrom || undefined,
      dateTo: filters.dateTo || undefined,
    });

    if (response.success) {
      return response.data;
    }

    return { items: [], totalCount: 0 };
  };

  const tableProps = useServerSideDataTable(fetchOrders, {
    queryKey: queryKeys.sales.orders(filters),
    dependencies: [filters],
  });

  const handleView = row => {
    navigate(`/orders/${row.id}`);
  };

  const handleEdit = row => {
    if (row.status === ORDER_STATUS.Pending) {
      navigate(`/orders/${row.id}/edit`);
    }
  };

  const handleFilterChange = (key, value) => {
    setFilters(prev => ({ ...prev, [key]: value }));
  };

  const clearFilters = () => {
    setFilters({
      status: '',
      customerId: '',
      dateFrom: '',
      dateTo: '',
    });
  };

  const hasActiveFilters =
    filters.status || filters.customerId || filters.dateFrom || filters.dateTo;

  const statusOptions = [
    { value: '', label: t(i18nKeyContainer.sales.orders.table.filters.allStatuses) },
    { value: 'Pending', label: t(i18nKeyContainer.sales.orders.status.pending) },
    { value: 'Confirmed', label: t(i18nKeyContainer.sales.orders.status.confirmed) },
    { value: 'InTransit', label: t(i18nKeyContainer.sales.orders.status.intransit) },
    { value: 'Shipped', label: t(i18nKeyContainer.sales.orders.status.shipped) },
    { value: 'Completed', label: t(i18nKeyContainer.sales.orders.status.completed) },
    { value: 'Cancelled', label: t(i18nKeyContainer.sales.orders.status.cancelled) },
    { value: 'Returned', label: t(i18nKeyContainer.sales.orders.status.returned) },
  ];

  return (
    <div className="space-y-4">
      {/* Filter Bar */}
      <div className="flex items-center justify-between">
        <Button
          variant="secondary"
          size="sm"
          LeftIcon={Filter}
          onClick={() => setShowFilters(!showFilters)}
        >
          {t(i18nKeyContainer.sales.orders.table.filters.status)}
          {hasActiveFilters && (
            <span className="ml-2 bg-blue-500 text-white text-xs px-1.5 py-0.5 rounded-full">
              !
            </span>
          )}
        </Button>

        {hasActiveFilters && (
          <Button variant="ghost" size="sm" LeftIcon={X} onClick={clearFilters}>
            {t(i18nKeyContainer.sales.orders.table.filters.clearFilters)}
          </Button>
        )}
      </div>

      {/* Expandable Filters */}
      {showFilters && (
        <div className="bg-gray-50 rounded-lg p-4 grid grid-cols-1 md:grid-cols-4 gap-4">
          {/* Status Filter */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              {t(i18nKeyContainer.sales.orders.table.filters.status)}
            </label>
            <select
              value={filters.status}
              onChange={e => handleFilterChange('status', e.target.value)}
              className="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm"
            >
              {statusOptions.map(opt => (
                <option key={opt.value} value={opt.value}>
                  {opt.label}
                </option>
              ))}
            </select>
          </div>

          {/* Date From */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              {t(i18nKeyContainer.sales.orders.table.filters.dateFrom)}
            </label>
            <input
              type="date"
              value={filters.dateFrom}
              onChange={e => handleFilterChange('dateFrom', e.target.value)}
              className="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm"
            />
          </div>

          {/* Date To */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              {t(i18nKeyContainer.sales.orders.table.filters.dateTo)}
            </label>
            <input
              type="date"
              value={filters.dateTo}
              onChange={e => handleFilterChange('dateTo', e.target.value)}
              className="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm"
            />
          </div>
        </div>
      )}

      {/* Data Table */}
      {tableProps.data.length === 0 && !tableProps.loading ? (
        <EmptyState hasFilters={hasActiveFilters} t={t} />
      ) : (
        <DataTable
          data={tableProps.data}
          columns={getColumns()}
          totalRows={tableProps.totalRows}
          pageIndex={tableProps.pageIndex}
          pageSize={tableProps.pageSize}
          onPageChange={tableProps.onPageChange}
          onPageSizeChange={tableProps.onPageSizeChange}
          onSortingChange={tableProps.onSortingChange}
          onFilterChange={tableProps.onFilterChange}
          loading={tableProps.loading}
          onView={handleView}
          onEdit={row => (row.status === ORDER_STATUS.Pending ? handleEdit(row) : null)}
          showEditButton={row => row.status === ORDER_STATUS.Pending}
        />
      )}
    </div>
  );
}

/**
 * Empty state component
 */
function EmptyState({ hasFilters, t }) {
  return (
    <div className="text-center py-12 bg-gray-50 rounded-lg border border-dashed border-gray-300">
      <ShoppingCart className="mx-auto h-12 w-12 text-gray-400" />
      <h3 className="mt-2 text-sm font-medium text-gray-900">
        {hasFilters
          ? t(i18nKeyContainer.sales.orders.table.emptyState.title)
          : t(i18nKeyContainer.sales.orders.table.emptyState.noFiltersTitle)}
      </h3>
      <p className="mt-1 text-sm text-gray-500">
        {hasFilters
          ? t(i18nKeyContainer.sales.orders.table.emptyState.description)
          : t(i18nKeyContainer.sales.orders.table.emptyState.noFiltersDescription)}
      </p>
    </div>
  );
}
