import { useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';
import {
  Truck,
  MapPin,
  Package,
  Clock,
  Calendar,
  User,
  ArrowRight,
} from 'lucide-react';
import { getStockTransferById } from '@features/inventory/services/stockTransferApi';
import { useTranslation } from 'react-i18next';
import i18nKeyContainer from '@shared/lib/i18n/keyContainer';
import { queryKeys } from '@shared/lib/queryKeys';

/**
 * ViewStockTransfer Component
 *
 * A comprehensive dialog component for displaying detailed stock transfer information in a tabbed interface.
 * Shows Transfer Details, Items, and History tabs based on backend data.
 *
 * Backend Response Structure (from TransferQueries):
 * {
 *   id: number,
 *   fromLocationId: number,
 *   fromLocationName: string,
 *   toLocationId: number,
 *   toLocationName: string,
 *   prodcutId: number,  // Note: typo in backend
 *   productName: string,
 *   status: string,  // "1"=Pending, "2"=In Transit, "3"=Completed, "4"=Cancelled
 *   quantity: number,
 *   createdAt: string (ISO date),
 *   createdByUserId: number,
 *   createdByUserName: string
 * }
 *
 * @param {Object} props - Component props
 * @param {boolean} props.open - Controls dialog visibility
 * @param {Function} props.onOpenChange - Callback when dialog open state changes
 * @param {number} props.transferId - Stock Transfer ID to display
 *
 * @example
 * ```jsx
 * const [viewDialogOpen, setViewDialogOpen] = useState(false);
 * const [selectedTransferId, setSelectedTransferId] = useState(null);
 *
 * <ViewStockTransfer
 *   open={viewDialogOpen}
 *   onOpenChange={setViewDialogOpen}
 *   transferId={selectedTransferId}
 * />
 * ```
 */
const ViewStockTransfer = ({ open, onOpenChange, transferId }) => {
  const { t, i18n } = useTranslation();
  const [activeTab, setActiveTab] = useState('details');
  const { data: transferResponse, isLoading: loading } = useQuery({
    queryKey: [...queryKeys.inventory.stockTransfers('list'), 'detail', transferId],
    queryFn: () => getStockTransferById(transferId),
    enabled: open && Boolean(transferId),
  });

  const transfer = transferResponse?.success ? transferResponse.data : null;

  const tabs = [
    {
      id: 'details',
      label: t(i18nKeyContainer.inventory.stockTransfers.view.tabs.transferDetails),
    },
    { id: 'items', label: t(i18nKeyContainer.inventory.stockTransfers.view.tabs.items) },
    {
      id: 'history',
      label: t(i18nKeyContainer.inventory.stockTransfers.view.tabs.history),
    },
  ];

  // Status mapping - backend returns status as number string
  const getStatusInfo = status => {
    // Backend returns: 1=Pending, 2=In Transit, 3=Completed, 4=Cancelled
    const statusMap = {
      1: { color: 'yellow', label: t(i18nKeyContainer.inventory.shared.status.pending) },
      2: {
        color: 'blue',
        label: t(i18nKeyContainer.inventory.shared.status.inTransit),
      },
      3: {
        color: 'green',
        label: t(i18nKeyContainer.inventory.shared.status.completed),
      },
      4: {
        color: 'red',
        label: t(i18nKeyContainer.inventory.shared.status.cancelled),
      },
      // Also support string versions
      Pending: {
        color: 'yellow',
        label: t(i18nKeyContainer.inventory.shared.status.pending),
      },
      'In Transit': {
        color: 'blue',
        label: t(i18nKeyContainer.inventory.shared.status.inTransit),
      },
      InTransit: {
        color: 'blue',
        label: t(i18nKeyContainer.inventory.shared.status.inTransit),
      },
      Completed: {
        color: 'green',
        label: t(i18nKeyContainer.inventory.shared.status.completed),
      },
      Cancelled: {
        color: 'red',
        label: t(i18nKeyContainer.inventory.shared.status.cancelled),
      },
    };
    return (
      statusMap[status] || {
        color: 'gray',
        label: status || t(i18nKeyContainer.inventory.shared.notSpecified),
      }
    );
  };

  if (loading || !transfer) {
    return (
      <Dialog open={open} onOpenChange={onOpenChange}>
        <DialogContent className='max-w-5xl'>
          <div className='flex items-center justify-center p-8'>
            <div className='text-gray-500'>
              {t(i18nKeyContainer.inventory.stockTransfers.view.loading)}
            </div>
          </div>
        </DialogContent>
      </Dialog>
    );
  }

  const statusInfo = getStatusInfo(transfer.status);

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className='max-w-5xl max-h-[90vh] overflow-hidden flex flex-col'>
        {/* Header */}
        <DialogHeader>
          <div className='flex items-center justify-between'>
            <div className='flex items-center gap-2'>
              <Truck className='w-5 h-5' />
              <DialogTitle className='text-xl'>
                {t(i18nKeyContainer.inventory.stockTransfers.view.title)}
              </DialogTitle>
              <span className='text-sm text-gray-500 font-normal'>
                {t(i18nKeyContainer.inventory.stockTransfers.view.transferCode, {
                  id: transfer.id.toString().padStart(3, '0'),
                })}
              </span>
            </div>
          </div>
        </DialogHeader>

        {/* Status Badge */}
        <div className='flex items-center gap-3 pb-2'>
          <span
            className={`inline-flex items-center gap-1 px-3 py-1 rounded-full text-xs font-medium ${
              statusInfo.color === 'green'
                ? 'bg-green-100 text-green-800'
                : statusInfo.color === 'blue'
                  ? 'bg-blue-100 text-blue-800'
                  : statusInfo.color === 'yellow'
                    ? 'bg-yellow-100 text-yellow-800'
                    : statusInfo.color === 'red'
                      ? 'bg-red-100 text-red-800'
                      : 'bg-gray-100 text-gray-800'
            }`}
          >
            {statusInfo.label}
          </span>
        </div>

        {/* Tabs Navigation */}
        <div className='flex border-b border-gray-200'>
          {tabs.map(tab => (
            <button
              key={tab.id}
              onClick={() => setActiveTab(tab.id)}
              className={`px-6 py-3 text-sm font-medium transition-colors ${
                activeTab === tab.id
                  ? 'border-b-2 border-blue-500 text-blue-600'
                  : 'text-gray-500 hover:text-gray-700'
              }`}
            >
              {tab.label}
            </button>
          ))}
        </div>

        {/* Tab Content */}
        <div className='flex-1 overflow-y-auto py-6'>
          {/* Transfer Details Tab */}
          {activeTab === 'details' && (
            <div className='space-y-6'>
              {/* Transfer Route */}
              <div>
                <h3 className='text-lg font-semibold mb-4 flex items-center gap-2'>
                  <MapPin className='w-5 h-5' />
                  {t(i18nKeyContainer.inventory.stockTransfers.view.sections.transferRoute)}
                </h3>

                {/* From and To Locations */}
                <div className='grid grid-cols-2 gap-6 mb-4'>
                  <div>
                    <label className='block text-sm font-medium text-gray-700 mb-2'>
                      {t(i18nKeyContainer.inventory.stockTransfers.view.fields.fromLocation)}{' '}
                      <span className='text-red-500'>
                        {t(i18nKeyContainer.inventory.shared.required)}
                      </span>
                    </label>
                    <div className='bg-gray-50 p-3 rounded-md'>
                      <p className='text-gray-900 font-medium'>
                        {transfer.fromLocationName ||
                          t(
                            i18nKeyContainer.inventory.stockTransfers.view.placeholders
                              .notSpecified
                          )}
                      </p>
                    </div>
                  </div>

                  <div>
                    <label className='block text-sm font-medium text-gray-700 mb-2'>
                      {t(i18nKeyContainer.inventory.stockTransfers.view.fields.toLocation)}{' '}
                      <span className='text-red-500'>
                        {t(i18nKeyContainer.inventory.shared.required)}
                      </span>
                    </label>
                    <div className='bg-gray-50 p-3 rounded-md'>
                      <p className='text-gray-900 font-medium'>
                        {transfer.toLocationName ||
                          t(
                            i18nKeyContainer.inventory.stockTransfers.view.placeholders
                              .notSpecified
                          )}
                      </p>
                    </div>
                  </div>
                </div>

                {/* Visual Route Display */}
                <div className='bg-blue-50 p-4 rounded-lg border border-blue-200'>
                  <div className='flex items-center justify-center gap-4'>
                    <div className='text-center'>
                      <p className='font-semibold text-blue-900'>
                        {transfer.fromLocationName ||
                          t(
                            i18nKeyContainer.inventory.stockTransfers.view.placeholders
                              .fromLocation
                          )}
                      </p>
                    </div>
                    <ArrowRight className='w-8 h-8 text-blue-600' />
                    <div className='text-center'>
                      <p className='font-semibold text-blue-900'>
                        {transfer.toLocationName ||
                          t(
                            i18nKeyContainer.inventory.stockTransfers.view.placeholders
                              .toLocation
                          )}
                      </p>
                    </div>
                  </div>
                </div>
              </div>

              {/* Product Information */}
              <div>
                <h3 className='text-lg font-semibold mb-4 flex items-center gap-2'>
                  <Package className='w-5 h-5' />
                  {t(
                    i18nKeyContainer.inventory.stockTransfers.view.sections
                      .productInformation
                  )}
                </h3>
                <div className='grid grid-cols-2 gap-6'>
                  <div>
                    <label className='block text-sm font-medium text-gray-700 mb-2'>
                      {t(i18nKeyContainer.inventory.stockTransfers.view.fields.product)}
                    </label>
                    <div className='bg-gray-50 p-3 rounded-md'>
                      <p className='text-gray-900 font-medium'>
                        {transfer.productName || '-'}
                      </p>
                    </div>
                  </div>
                  <div>
                    <label className='block text-sm font-medium text-gray-700 mb-2'>
                      {t(i18nKeyContainer.inventory.stockTransfers.view.fields.quantity)}
                    </label>
                    <div className='bg-gray-50 p-3 rounded-md'>
                      <p className='text-gray-900 font-medium'>
                        {transfer.quantity
                          ? transfer.quantity.toFixed(2)
                          : '0.00'}{' '}
                        {t(i18nKeyContainer.inventory.stockTransfers.view.labels.units)}
                      </p>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          )}

          {/* Items Tab */}
          {activeTab === 'items' && (
            <div className='space-y-6'>
              <h3 className='text-lg font-semibold mb-4 flex items-center gap-2'>
                <Package className='w-5 h-5' />
                {t(i18nKeyContainer.inventory.stockTransfers.view.sections.transferItem)}
              </h3>

              {/* Single Item Display */}
              <div className='border border-gray-200 rounded-lg p-4 space-y-4'>
                <div className='flex items-center justify-between'>
                  <h4 className='font-semibold text-gray-900'>
                    {t(i18nKeyContainer.inventory.stockTransfers.view.placeholders.itemLabel, {
                      index: 1,
                    })}
                  </h4>
                  <span
                    className={`inline-flex items-center gap-1 px-3 py-1 rounded-full text-xs font-medium ${
                      statusInfo.color === 'green'
                        ? 'bg-green-100 text-green-800'
                        : 'bg-yellow-100 text-yellow-800'
                    }`}
                  >
                    {t(
                      i18nKeyContainer.inventory.stockTransfers.view.labels
                        .statusWithValue,
                      {
                        status: statusInfo.label,
                      }
                    )}
                  </span>
                </div>

                <div className='grid grid-cols-2 gap-4'>
                  <div>
                    <label className='block text-sm font-medium text-gray-700 mb-1'>
                      {t(i18nKeyContainer.inventory.stockTransfers.view.fields.product)}{' '}
                      <span className='text-red-500'>
                        {t(i18nKeyContainer.inventory.shared.required)}
                      </span>
                    </label>
                    <p className='text-gray-900'>
                      {transfer.productName || '-'}
                    </p>
                  </div>

                  <div>
                    <label className='block text-sm font-medium text-gray-700 mb-1'>
                      {t(i18nKeyContainer.inventory.stockTransfers.view.fields.quantity)}{' '}
                      <span className='text-red-500'>
                        {t(i18nKeyContainer.inventory.shared.required)}
                      </span>
                    </label>
                    <p className='text-gray-900'>
                      {transfer.quantity
                        ? transfer.quantity.toFixed(2)
                        : '0.00'}{' '}
                      {t(i18nKeyContainer.inventory.stockTransfers.view.labels.units)}
                    </p>
                  </div>
                </div>
              </div>

              {/* Transfer Summary */}
              <div className='border-t pt-4 mt-6'>
                <div className='flex justify-between items-center'>
                  <div>
                    <p className='text-sm font-medium text-gray-700'>
                      {t(
                        i18nKeyContainer.inventory.stockTransfers.view.sections
                          .transferSummary
                      )}
                    </p>
                    <p className='text-sm text-gray-500'>
                      {t(i18nKeyContainer.inventory.stockTransfers.view.labels.oneProduct)},{' '}
                      {t(i18nKeyContainer.inventory.stockTransfers.view.labels.totalUnits, {
                        count: transfer.quantity ? transfer.quantity.toFixed(2) : '0.00',
                      })}
                    </p>
                  </div>
                </div>
              </div>
            </div>
          )}

          {/* History Tab */}
          {activeTab === 'history' && (
            <div className='space-y-6'>
              <h3 className='text-lg font-semibold mb-4 flex items-center gap-2'>
                <Clock className='w-5 h-5' />
                {t(i18nKeyContainer.inventory.stockTransfers.view.sections.transferHistory)}
              </h3>

              {/* Creation Details */}
              <div className='bg-blue-50 p-4 rounded-lg space-y-3'>
                <h4 className='font-semibold text-blue-900 flex items-center gap-2'>
                  <Calendar className='w-4 h-4' />
                  {t(i18nKeyContainer.inventory.stockTransfers.view.sections.created)}
                </h4>
                <div className='grid grid-cols-2 gap-4'>
                  <div>
                    <label className='block text-sm font-medium text-blue-700 mb-1'>
                      {t(i18nKeyContainer.inventory.stockTransfers.view.fields.dateTime)}
                    </label>
                    <p className='text-blue-900 text-sm'>
                      {transfer.createdAt
                        ? new Date(transfer.createdAt).toLocaleString(
                            i18n.resolvedLanguage || i18n.language || 'en',
                            {
                            year: 'numeric',
                            month: 'long',
                            day: 'numeric',
                            hour: '2-digit',
                            minute: '2-digit',
                            }
                          )
                        : '-'}
                    </p>
                  </div>
                  <div>
                    <label className='text-sm font-medium text-blue-700 mb-1 flex items-center gap-1'>
                      <User className='w-3 h-3' />
                      {t(i18nKeyContainer.inventory.stockTransfers.view.fields.user)}
                    </label>
                    <p className='text-blue-900 text-sm'>
                      {transfer.createdByUserName ||
                        t(i18nKeyContainer.inventory.shared.systemUser)}
                    </p>
                  </div>
                </div>
              </div>

              {/* Transfer Information */}
              <div className='border-t pt-4'>
                <h4 className='font-semibold text-gray-900 mb-3'>
                  {t(
                    i18nKeyContainer.inventory.stockTransfers.view.sections
                      .transferInformation
                  )}
                </h4>
                <div className='grid grid-cols-2 gap-4'>
                  <div>
                    <label className='block text-sm font-medium text-gray-700 mb-1'>
                      {t(i18nKeyContainer.inventory.stockTransfers.view.fields.transferId)}
                    </label>
                    <p className='text-gray-900 font-mono text-sm'>
                      {transfer.id}
                    </p>
                  </div>
                  <div>
                    <label className='block text-sm font-medium text-gray-700 mb-1'>
                      {t(
                        i18nKeyContainer.inventory.stockTransfers.view.fields
                          .currentStatus
                      )}
                    </label>
                    <p className='text-gray-900'>{statusInfo.label}</p>
                  </div>
                  <div>
                    <label className='block text-sm font-medium text-gray-700 mb-1'>
                      {t(
                        i18nKeyContainer.inventory.stockTransfers.view.fields
                          .fromLocationId
                      )}
                    </label>
                    <p className='text-gray-900 font-mono text-sm'>
                      {transfer.fromLocationId || '-'}
                    </p>
                  </div>
                  <div>
                    <label className='block text-sm font-medium text-gray-700 mb-1'>
                      {t(
                        i18nKeyContainer.inventory.stockTransfers.view.fields
                          .toLocationId
                      )}
                    </label>
                    <p className='text-gray-900 font-mono text-sm'>
                      {transfer.toLocationId || '-'}
                    </p>
                  </div>
                  <div>
                    <label className='block text-sm font-medium text-gray-700 mb-1'>
                      {t(i18nKeyContainer.inventory.stockTransfers.view.fields.productId)}
                    </label>
                    <p className='text-gray-900 font-mono text-sm'>
                      {transfer.prodcutId || '-'}
                    </p>
                  </div>
                  <div>
                    <label className='block text-sm font-medium text-gray-700 mb-1'>
                      {t(
                        i18nKeyContainer.inventory.stockTransfers.view.fields
                          .createdByUserId
                      )}
                    </label>
                    <p className='text-gray-900 font-mono text-sm'>
                      {transfer.createdByUserId || '-'}
                    </p>
                  </div>
                </div>
              </div>
            </div>
          )}
        </div>

        {/* Footer Actions */}
        <div className='flex justify-end gap-2 border-t pt-4'>
          <button
            onClick={() => onOpenChange(false)}
            className='px-4 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded-md hover:bg-gray-50 transition-colors'
          >
            {t(i18nKeyContainer.inventory.shared.close)}
          </button>
        </div>
      </DialogContent>
    </Dialog>
  );
};

export default ViewStockTransfer;
