import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import {
  ShoppingCart,
  Filter,
  X,
  Check,
  Truck,
  Package,
  XCircle,
  RotateCcw,
} from 'lucide-react';
import DataTable from '@components/DataTable/DataTable';
import useServerSideDataTable from '@shared/hooks/useServerSideDataTable';
import Button from '@components/Buttons/Button';
import ConfirmationDialog from '@components/ui/ConfirmationDialog';
import StatusBadge from './StatusBadge';
import PaymentStatusBadge from './PaymentStatusBadge';
import { useOrderTransition } from '@features/sales/hooks/useOrders';
import { getOrders } from '@features/sales/services/salesOrderApi';
import {
  ACTION_CONFIG,
  ORDER_STATUS,
  STATUS_TRANSITIONS,
} from '@features/sales/utils/orderConstants';
import { queryKeys } from '@shared/lib/queryKeys';
import i18nKeyContainer from '@shared/lib/i18n/keyContainer';
import { formatDzdCurrency } from '@shared/utils/currencyFormatter';

/**
 * OrdersDataTable Component
 *
 * Server-side paginated table for orders with filters.
 *
 * @param {Object} props - Component props
 * @param {Object} props.initialFilters - Optional default filters
 */
export default function OrdersDataTable({ initialFilters = {} }) {
  const { t, i18n } = useTranslation();
  const navigate = useNavigate();
  const activeLocale = i18n.resolvedLanguage || i18n.language || 'en';
  const transition = useOrderTransition();
  const [filters, setFilters] = useState({
    status: initialFilters.status || '',
    customerId: initialFilters.customerId || '',
    dateFrom: initialFilters.dateFrom || '',
    dateTo: initialFilters.dateTo || '',
  });
  const [showFilters, setShowFilters] = useState(false);
  const [confirmAction, setConfirmAction] = useState(null);
  const [shipOrder, setShipOrder] = useState(null);
  const [trackingNumber, setTrackingNumber] = useState('');

  const getRowQuickActions = status => {
    const allowedActions = STATUS_TRANSITIONS[status] || [];
    if (allowedActions.length === 0) return [];

    const primaryActionMap = {
      [ORDER_STATUS.Pending]: 'confirm',
      [ORDER_STATUS.Confirmed]: 'transit',
      [ORDER_STATUS.InTransit]: 'ship',
      [ORDER_STATUS.Shipped]: 'complete',
      [ORDER_STATUS.Completed]: 'return',
    };

    const quickActions = [];
    const primaryAction = primaryActionMap[status];
    if (primaryAction && allowedActions.includes(primaryAction)) {
      quickActions.push(primaryAction);
    }

    if (allowedActions.includes('cancel')) {
      quickActions.push('cancel');
    }

    return quickActions;
  };

  const getActionIcon = action => {
    switch (action) {
      case 'confirm':
        return Check;
      case 'transit':
        return Truck;
      case 'ship':
        return Package;
      case 'complete':
        return Check;
      case 'cancel':
        return XCircle;
      case 'return':
        return RotateCcw;
      default:
        return null;
    }
  };

  const getDialogConfig = action => {
    const configs = {
      confirm: {
        title: t(i18nKeyContainer.sales.orders.confirmDialog.confirmTitle),
        message: t(i18nKeyContainer.sales.orders.confirmDialog.confirmMessage),
        type: 'update',
      },
      transit: {
        title: t(i18nKeyContainer.sales.orders.confirmDialog.transitTitle),
        message: t(i18nKeyContainer.sales.orders.confirmDialog.transitMessage),
        type: 'update',
      },
      complete: {
        title: t(i18nKeyContainer.sales.orders.confirmDialog.completeTitle),
        message: t(i18nKeyContainer.sales.orders.confirmDialog.completeMessage),
        type: 'create',
      },
      cancel: {
        title: t(i18nKeyContainer.sales.orders.confirmDialog.cancelTitle),
        message: t(i18nKeyContainer.sales.orders.confirmDialog.cancelMessage),
        type: 'delete',
      },
      return: {
        title: t(i18nKeyContainer.sales.orders.confirmDialog.returnTitle),
        message: t(i18nKeyContainer.sales.orders.confirmDialog.returnMessage),
        type: 'delete',
      },
    };

    return configs[action] || { title: '', message: '', type: 'update' };
  };

  const handleTransitionClick = (row, action) => {
    if (action === 'ship') {
      setShipOrder(row);
      setTrackingNumber('');
      return;
    }

    setConfirmAction({ orderId: row.id, action });
  };

  const handleConfirmTransition = async () => {
    if (!confirmAction) return;

    try {
      await transition.mutateAsync({
        orderId: confirmAction.orderId,
        action: confirmAction.action,
      });
      setConfirmAction(null);
    } catch {
      // Error handling is done in hook
    }
  };

  const handleShipTransition = async () => {
    if (!shipOrder) return;

    try {
      await transition.mutateAsync({
        orderId: shipOrder.id,
        action: 'ship',
        payload: trackingNumber ? { trackingNumber } : {},
      });
      setShipOrder(null);
      setTrackingNumber('');
    } catch {
      // Error handling is done in hook
    }
  };

  const getTableActions = row => {
    const quickActions = getRowQuickActions(row.status);

    return quickActions.map(action => {
      const config = ACTION_CONFIG[action];
      const Icon = getActionIcon(action);

      return {
        key: `status-${action}`,
        label: t(config.labelKey),
        icon: Icon,
        variant: config.variant,
        disabled: transition.isPending,
        onClick: () => handleTransitionClick(row, action),
      };
    });
  };

  const getColumns = () => [
    {
      accessorKey: 'orderNumber',
      header: t(i18nKeyContainer.sales.orders.table.columns.orderNumber),
      cell: ({ row }) => (
        <button
          onClick={() => navigate(`/sales-orders/${row.original.id}`)}
          className='font-medium text-blue-600 hover:text-blue-800 hover:underline'
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
          {row.original.customerName ||
            t(i18nKeyContainer.sales.orders.table.walkIn)}
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
      accessorKey: 'items',
      header: t(i18nKeyContainer.sales.orders.table.columns.items),
      cell: ({ row }) => {
        const rawItems = row.original.items ?? row.original.itemCount ?? 0;
        if (Array.isArray(rawItems)) {
          return rawItems.length;
        }

        const count = Number(rawItems);
        return Number.isFinite(count) ? count : 0;
      },
    },
    {
      accessorKey: 'totalAmount',
      header: t(i18nKeyContainer.sales.orders.table.columns.total),
      cell: ({ row }) => {
        const rawTotal = row.original.totalAmount ?? row.original.total ?? 0;
        const total = Number(rawTotal);
        return formatDzdCurrency(Number.isFinite(total) ? total : 0, {
          locale: activeLocale,
        });
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

  const fetchOrders = async ({ page, pageSize }) => {
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
    navigate(`/sales-orders/${row.id}`);
  };

  const handleEdit = row => {
    if (row.status === ORDER_STATUS.Pending) {
      navigate(`/sales-orders/${row.id}/edit`);
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
    {
      value: '',
      label: t(i18nKeyContainer.sales.orders.table.filters.allStatuses),
    },
    {
      value: 'Pending',
      label: t(i18nKeyContainer.sales.orders.status.pending),
    },
    {
      value: 'Confirmed',
      label: t(i18nKeyContainer.sales.orders.status.confirmed),
    },
    {
      value: 'InTransit',
      label: t(i18nKeyContainer.sales.orders.status.intransit),
    },
    {
      value: 'Shipped',
      label: t(i18nKeyContainer.sales.orders.status.shipped),
    },
    {
      value: 'Completed',
      label: t(i18nKeyContainer.sales.orders.status.completed),
    },
    {
      value: 'Cancelled',
      label: t(i18nKeyContainer.sales.orders.status.cancelled),
    },
    {
      value: 'Returned',
      label: t(i18nKeyContainer.sales.orders.status.returned),
    },
  ];

  return (
    <div className='space-y-4'>
      {/* Filter Bar */}
      <div className='flex items-center justify-between'>
        <Button
          variant='secondary'
          size='sm'
          LeftIcon={Filter}
          onClick={() => setShowFilters(!showFilters)}
        >
          {t(i18nKeyContainer.sales.orders.table.filters.status)}
          {hasActiveFilters && (
            <span className='ml-2 bg-blue-500 text-white text-xs px-1.5 py-0.5 rounded-full'>
              !
            </span>
          )}
        </Button>

        {hasActiveFilters && (
          <Button
            variant='secondary'
            size='sm'
            LeftIcon={X}
            onClick={clearFilters}
          >
            {t(i18nKeyContainer.sales.orders.table.filters.clearFilters)}
          </Button>
        )}
      </div>

      {/* Expandable Filters */}
      {showFilters && (
        <div className='bg-gray-50 rounded-lg p-4 grid grid-cols-1 md:grid-cols-4 gap-4'>
          {/* Status Filter */}
          <div>
            <label className='block text-sm font-medium text-gray-700 mb-1'>
              {t(i18nKeyContainer.sales.orders.table.filters.status)}
            </label>
            <select
              value={filters.status}
              onChange={e => handleFilterChange('status', e.target.value)}
              className='block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm'
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
            <label className='block text-sm font-medium text-gray-700 mb-1'>
              {t(i18nKeyContainer.sales.orders.table.filters.dateFrom)}
            </label>
            <input
              type='date'
              value={filters.dateFrom}
              onChange={e => handleFilterChange('dateFrom', e.target.value)}
              className='block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm'
            />
          </div>

          {/* Date To */}
          <div>
            <label className='block text-sm font-medium text-gray-700 mb-1'>
              {t(i18nKeyContainer.sales.orders.table.filters.dateTo)}
            </label>
            <input
              type='date'
              value={filters.dateTo}
              onChange={e => handleFilterChange('dateTo', e.target.value)}
              className='block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm'
            />
          </div>
        </div>
      )}

      {/* Data Table */}
      {tableProps.data.length === 0 && !tableProps.loading ? (
        <EmptyState hasFilters={hasActiveFilters} t={t} />
      ) : (
        <>
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
            onEdit={row =>
              row.status === ORDER_STATUS.Pending ? handleEdit(row) : null
            }
            customActions={getTableActions}
            showEditButton={row => row.status === ORDER_STATUS.Pending}
          />

          {confirmAction && (
            <ConfirmationDialog
              isOpen={!!confirmAction}
              onClose={() => setConfirmAction(null)}
              onConfirm={handleConfirmTransition}
              type={getDialogConfig(confirmAction.action).type}
              title={getDialogConfig(confirmAction.action).title}
              message={getDialogConfig(confirmAction.action).message}
              confirmText={t(i18nKeyContainer.sales.shared.confirm)}
              loading={transition.isPending}
            />
          )}

          {shipOrder && (
            <div className='fixed inset-0 z-[9999] flex items-center justify-center bg-black/50 backdrop-blur-sm'>
              <div className='relative w-full max-w-md mx-4 bg-white rounded-xl shadow-2xl'>
                <div className='bg-purple-50 border-b border-purple-200 rounded-t-xl p-6'>
                  <div className='flex items-start gap-4'>
                    <div className='bg-purple-100 rounded-full p-3 flex-shrink-0'>
                      <Package className='w-6 h-6 text-purple-600' />
                    </div>
                    <div className='flex-1 min-w-0'>
                      <h2 className='text-xl font-semibold text-gray-900 mb-1'>
                        {t(
                          i18nKeyContainer.sales.orders.confirmDialog.shipTitle
                        )}
                      </h2>
                    </div>
                  </div>
                </div>

                <div className='p-6 space-y-4'>
                  <p className='text-sm text-gray-600'>
                    {t(i18nKeyContainer.sales.orders.confirmDialog.shipMessage)}
                  </p>
                  <div>
                    <label className='block text-sm font-medium text-gray-700 mb-1'>
                      {t(
                        i18nKeyContainer.sales.orders.confirmDialog
                          .trackingNumber
                      )}{' '}
                      <span className='text-gray-400'>
                        {t(i18nKeyContainer.sales.shared.optional)}
                      </span>
                    </label>
                    <input
                      type='text'
                      value={trackingNumber}
                      onChange={e => setTrackingNumber(e.target.value)}
                      placeholder={t(
                        i18nKeyContainer.sales.orders.confirmDialog
                          .trackingNumberPlaceholder
                      )}
                      className='block w-full rounded-md border-gray-300 shadow-sm focus:border-purple-500 focus:ring-purple-500 sm:text-sm'
                    />
                  </div>
                </div>

                <div className='flex items-center justify-end gap-3 px-6 py-4 bg-gray-50 rounded-b-xl border-t border-gray-200'>
                  <button
                    onClick={() => {
                      setShipOrder(null);
                      setTrackingNumber('');
                    }}
                    disabled={transition.isPending}
                    className='px-4 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded-lg hover:bg-gray-50 transition-colors disabled:opacity-50'
                    type='button'
                  >
                    {t(i18nKeyContainer.sales.shared.cancel)}
                  </button>
                  <button
                    onClick={handleShipTransition}
                    disabled={transition.isPending}
                    className='px-4 py-2 text-sm font-medium text-white bg-purple-600 rounded-lg hover:bg-purple-700 transition-colors disabled:opacity-50 flex items-center gap-2'
                    type='button'
                  >
                    {transition.isPending && (
                      <svg
                        className='animate-spin h-4 w-4 text-white'
                        xmlns='http://www.w3.org/2000/svg'
                        fill='none'
                        viewBox='0 0 24 24'
                      >
                        <circle
                          className='opacity-25'
                          cx='12'
                          cy='12'
                          r='10'
                          stroke='currentColor'
                          strokeWidth='4'
                        />
                        <path
                          className='opacity-75'
                          fill='currentColor'
                          d='M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z'
                        />
                      </svg>
                    )}
                    {t(i18nKeyContainer.sales.orders.actions.ship)}
                  </button>
                </div>
              </div>
            </div>
          )}
        </>
      )}
    </div>
  );
}

/**
 * Empty state component
 */
function EmptyState({ hasFilters, t }) {
  return (
    <div className='text-center py-12 bg-gray-50 rounded-lg border border-dashed border-gray-300'>
      <ShoppingCart className='mx-auto h-12 w-12 text-gray-400' />
      <h3 className='mt-2 text-sm font-medium text-gray-900'>
        {hasFilters
          ? t(i18nKeyContainer.sales.orders.table.emptyState.title)
          : t(i18nKeyContainer.sales.orders.table.emptyState.noFiltersTitle)}
      </h3>
      <p className='mt-1 text-sm text-gray-500'>
        {hasFilters
          ? t(i18nKeyContainer.sales.orders.table.emptyState.description)
          : t(
              i18nKeyContainer.sales.orders.table.emptyState
                .noFiltersDescription
            )}
      </p>
    </div>
  );
}
