import { useState, useEffect } from 'react';
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
import { getStockTransferById } from '@/services/stock/stockTransferService';

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
  const [activeTab, setActiveTab] = useState('details');
  const [transfer, setTransfer] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchTransfer = async () => {
      if (transferId && open) {
        setLoading(true);
        try {
          const response = await getStockTransferById(transferId);
          if (response.success) {
            setTransfer(response.data);
          }
        } catch (error) {
          console.error('Error fetching stock transfer:', error);
        } finally {
          setLoading(false);
        }
      }
    };
    fetchTransfer();
  }, [transferId, open]);

  const tabs = [
    { id: 'details', label: 'Transfer Details' },
    { id: 'items', label: 'Items' },
    { id: 'history', label: 'History' },
  ];

  // Status mapping - backend returns status as number string
  const getStatusInfo = status => {
    // Backend returns: 1=Pending, 2=In Transit, 3=Completed, 4=Cancelled
    const statusMap = {
      1: { color: 'yellow', label: 'Pending' },
      2: { color: 'blue', label: 'In Transit' },
      3: { color: 'green', label: 'Completed' },
      4: { color: 'red', label: 'Cancelled' },
      // Also support string versions
      Pending: { color: 'yellow', label: 'Pending' },
      'In Transit': { color: 'blue', label: 'In Transit' },
      Completed: { color: 'green', label: 'Completed' },
      Cancelled: { color: 'red', label: 'Cancelled' },
    };
    return statusMap[status] || { color: 'gray', label: status || 'Unknown' };
  };

  if (loading || !transfer) {
    return (
      <Dialog open={open} onOpenChange={onOpenChange}>
        <DialogContent className='max-w-5xl'>
          <div className='flex items-center justify-center p-8'>
            <div className='text-gray-500'>Loading...</div>
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
                Stock Transfer Details
              </DialogTitle>
              <span className='text-sm text-gray-500 font-normal'>
                TR{transfer.id.toString().padStart(3, '0')}
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
                  Transfer Route
                </h3>

                {/* From and To Locations */}
                <div className='grid grid-cols-2 gap-6 mb-4'>
                  <div>
                    <label className='block text-sm font-medium text-gray-700 mb-2'>
                      From Location <span className='text-red-500'>*</span>
                    </label>
                    <div className='bg-gray-50 p-3 rounded-md'>
                      <p className='text-gray-900 font-medium'>
                        {transfer.fromLocationName || 'Not specified'}
                      </p>
                    </div>
                  </div>

                  <div>
                    <label className='block text-sm font-medium text-gray-700 mb-2'>
                      To Location <span className='text-red-500'>*</span>
                    </label>
                    <div className='bg-gray-50 p-3 rounded-md'>
                      <p className='text-gray-900 font-medium'>
                        {transfer.toLocationName || 'Not specified'}
                      </p>
                    </div>
                  </div>
                </div>

                {/* Visual Route Display */}
                <div className='bg-blue-50 p-4 rounded-lg border border-blue-200'>
                  <div className='flex items-center justify-center gap-4'>
                    <div className='text-center'>
                      <p className='font-semibold text-blue-900'>
                        {transfer.fromLocationName || 'From Location'}
                      </p>
                    </div>
                    <ArrowRight className='w-8 h-8 text-blue-600' />
                    <div className='text-center'>
                      <p className='font-semibold text-blue-900'>
                        {transfer.toLocationName || 'To Location'}
                      </p>
                    </div>
                  </div>
                </div>
              </div>

              {/* Product Information */}
              <div>
                <h3 className='text-lg font-semibold mb-4 flex items-center gap-2'>
                  <Package className='w-5 h-5' />
                  Product Information
                </h3>
                <div className='grid grid-cols-2 gap-6'>
                  <div>
                    <label className='block text-sm font-medium text-gray-700 mb-2'>
                      Product
                    </label>
                    <div className='bg-gray-50 p-3 rounded-md'>
                      <p className='text-gray-900 font-medium'>
                        {transfer.productName || '-'}
                      </p>
                    </div>
                  </div>
                  <div>
                    <label className='block text-sm font-medium text-gray-700 mb-2'>
                      Quantity
                    </label>
                    <div className='bg-gray-50 p-3 rounded-md'>
                      <p className='text-gray-900 font-medium'>
                        {transfer.quantity
                          ? transfer.quantity.toFixed(2)
                          : '0.00'}{' '}
                        units
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
                Transfer Item
              </h3>

              {/* Single Item Display */}
              <div className='border border-gray-200 rounded-lg p-4 space-y-4'>
                <div className='flex items-center justify-between'>
                  <h4 className='font-semibold text-gray-900'>Item 1</h4>
                  <span
                    className={`inline-flex items-center gap-1 px-3 py-1 rounded-full text-xs font-medium ${
                      statusInfo.color === 'green'
                        ? 'bg-green-100 text-green-800'
                        : 'bg-yellow-100 text-yellow-800'
                    }`}
                  >
                    Status: {statusInfo.label}
                  </span>
                </div>

                <div className='grid grid-cols-2 gap-4'>
                  <div>
                    <label className='block text-sm font-medium text-gray-700 mb-1'>
                      Product <span className='text-red-500'>*</span>
                    </label>
                    <p className='text-gray-900'>
                      {transfer.productName || '-'}
                    </p>
                  </div>

                  <div>
                    <label className='block text-sm font-medium text-gray-700 mb-1'>
                      Quantity <span className='text-red-500'>*</span>
                    </label>
                    <p className='text-gray-900'>
                      {transfer.quantity
                        ? transfer.quantity.toFixed(2)
                        : '0.00'}{' '}
                      units
                    </p>
                  </div>
                </div>
              </div>

              {/* Transfer Summary */}
              <div className='border-t pt-4 mt-6'>
                <div className='flex justify-between items-center'>
                  <div>
                    <p className='text-sm font-medium text-gray-700'>
                      Transfer Summary
                    </p>
                    <p className='text-sm text-gray-500'>
                      1 product,{' '}
                      {transfer.quantity
                        ? transfer.quantity.toFixed(2)
                        : '0.00'}{' '}
                      total units
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
                Transfer History
              </h3>

              {/* Creation Details */}
              <div className='bg-blue-50 p-4 rounded-lg space-y-3'>
                <h4 className='font-semibold text-blue-900 flex items-center gap-2'>
                  <Calendar className='w-4 h-4' />
                  Created
                </h4>
                <div className='grid grid-cols-2 gap-4'>
                  <div>
                    <label className='block text-sm font-medium text-blue-700 mb-1'>
                      Date & Time
                    </label>
                    <p className='text-blue-900 text-sm'>
                      {transfer.createdAt
                        ? new Date(transfer.createdAt).toLocaleString('en-US', {
                            year: 'numeric',
                            month: 'long',
                            day: 'numeric',
                            hour: '2-digit',
                            minute: '2-digit',
                          })
                        : '-'}
                    </p>
                  </div>
                  <div>
                    <label className='text-sm font-medium text-blue-700 mb-1 flex items-center gap-1'>
                      <User className='w-3 h-3' />
                      User
                    </label>
                    <p className='text-blue-900 text-sm'>
                      {transfer.createdByUserName || 'System User'}
                    </p>
                  </div>
                </div>
              </div>

              {/* Transfer Information */}
              <div className='border-t pt-4'>
                <h4 className='font-semibold text-gray-900 mb-3'>
                  Transfer Information
                </h4>
                <div className='grid grid-cols-2 gap-4'>
                  <div>
                    <label className='block text-sm font-medium text-gray-700 mb-1'>
                      Transfer ID
                    </label>
                    <p className='text-gray-900 font-mono text-sm'>
                      {transfer.id}
                    </p>
                  </div>
                  <div>
                    <label className='block text-sm font-medium text-gray-700 mb-1'>
                      Current Status
                    </label>
                    <p className='text-gray-900'>{statusInfo.label}</p>
                  </div>
                  <div>
                    <label className='block text-sm font-medium text-gray-700 mb-1'>
                      From Location ID
                    </label>
                    <p className='text-gray-900 font-mono text-sm'>
                      {transfer.fromLocationId || '-'}
                    </p>
                  </div>
                  <div>
                    <label className='block text-sm font-medium text-gray-700 mb-1'>
                      To Location ID
                    </label>
                    <p className='text-gray-900 font-mono text-sm'>
                      {transfer.toLocationId || '-'}
                    </p>
                  </div>
                  <div>
                    <label className='block text-sm font-medium text-gray-700 mb-1'>
                      Product ID
                    </label>
                    <p className='text-gray-900 font-mono text-sm'>
                      {transfer.prodcutId || '-'}
                    </p>
                  </div>
                  <div>
                    <label className='block text-sm font-medium text-gray-700 mb-1'>
                      Created By User ID
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
            Close
          </button>
        </div>
      </DialogContent>
    </Dialog>
  );
};

export default ViewStockTransfer;
