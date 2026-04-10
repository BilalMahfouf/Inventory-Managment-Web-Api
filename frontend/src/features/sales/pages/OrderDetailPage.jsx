import { useParams, useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import {
  ArrowLeft,
  Calendar,
  User,
  FileText,
  Package,
  MapPin,
  Truck,
} from 'lucide-react';
import PageHeader from '@components/ui/PageHeader';
import Button from '@components/Buttons/Button';
import StatusBadge from '@features/sales/components/StatusBadge';
import PaymentStatusBadge from '@features/sales/components/PaymentStatusBadge';
import OrderActionBar from '@features/sales/components/OrderActionBar';
import { useOrder } from '@features/sales/hooks/useOrders';
import { ORDER_STATUS } from '@features/sales/utils/orderConstants';
import i18nKeyContainer from '@shared/lib/i18n/keyContainer';

/**
 * OrderDetailPage Component
 *
 * View page for displaying order details and allowing status transitions.
 *
 * @route /sales-orders/:id
 */
export default function OrderDetailPage() {
  const { t } = useTranslation();
  const { id } = useParams();
  const navigate = useNavigate();

  const { data: orderResponse, isLoading, error } = useOrder(id);

  const order = orderResponse?.success ? orderResponse.data : null;

  if (isLoading) {
    return <OrderDetailSkeleton />;
  }

  if (error || !order) {
    return (
      <div className='max-w-4xl mx-auto'>
        <div className='flex items-center gap-4 mb-6'>
          <Button
            variant='ghost'
            size='sm'
            LeftIcon={ArrowLeft}
            onClick={() => navigate('/sales-orders')}
          >
            {t(i18nKeyContainer.sales.shared.back)}
          </Button>
        </div>
        <div className='bg-white rounded-lg shadow-sm border border-gray-200 p-12 text-center'>
          <Package className='mx-auto h-12 w-12 text-gray-400' />
          <h3 className='mt-2 text-lg font-medium text-gray-900'>
            {t(i18nKeyContainer.sales.orders.detail.notFound)}
          </h3>
          <p className='mt-1 text-sm text-gray-500'>
            {t(i18nKeyContainer.sales.orders.detail.notFoundDescription)}
          </p>
        </div>
      </div>
    );
  }

  const canEdit = order.status === ORDER_STATUS.Pending;
  const orderTotal = Number(order.totalAmount ?? order.total ?? 0);

  return (
    <div className='max-w-4xl mx-auto'>
      {/* Header */}
      <div className='flex flex-col md:flex-row md:items-center md:justify-between mb-6 gap-4'>
        <div className='flex items-center gap-4'>
          <Button
            variant='ghost'
            size='sm'
            LeftIcon={ArrowLeft}
            onClick={() => navigate('/sales-orders')}
          >
            {t(i18nKeyContainer.sales.shared.back)}
          </Button>
          <div className='flex items-center gap-3'>
            <PageHeader
              title={t(i18nKeyContainer.sales.orders.detail.title, {
                orderNumber: order.orderNumber || order.id,
              })}
              description=''
            />
            <StatusBadge status={order.status} />
          </div>
        </div>

        <div className='flex items-center gap-2'>
          {canEdit && (
            <Button
              variant='secondary'
              size='sm'
              onClick={() => navigate(`/sales-orders/${id}/edit`)}
            >
              {t(i18nKeyContainer.sales.orders.actions.edit)}
            </Button>
          )}
        </div>
      </div>

      {/* Action Bar */}
      <div className='bg-white rounded-lg shadow-sm border border-gray-200 p-4 mb-6'>
        <OrderActionBar order={order} />
      </div>

      {/* Order Information */}
      <div className='bg-white rounded-lg shadow-sm border border-gray-200 p-6 mb-6'>
        <h3 className='text-lg font-medium text-gray-900 mb-4'>
          {t(i18nKeyContainer.sales.orders.detail.orderInfo)}
        </h3>
        <div className='grid grid-cols-1 md:grid-cols-2 gap-6'>
          {/* Customer */}
          <div className='flex items-start gap-3'>
            <div className='p-2 bg-gray-100 rounded-lg'>
              <User className='h-5 w-5 text-gray-600' />
            </div>
            <div>
              <p className='text-sm text-gray-500'>
                {t(i18nKeyContainer.sales.orders.detail.customer)}
              </p>
              <p className='font-medium'>
                {order.customerName || (
                  <span className='text-gray-500 italic'>
                    {t(i18nKeyContainer.sales.orders.table.walkIn)}
                  </span>
                )}
              </p>
            </div>
          </div>

          {/* Order Date */}
          <div className='flex items-start gap-3'>
            <div className='p-2 bg-gray-100 rounded-lg'>
              <Calendar className='h-5 w-5 text-gray-600' />
            </div>
            <div>
              <p className='text-sm text-gray-500'>
                {t(i18nKeyContainer.sales.orders.detail.orderDate)}
              </p>
              <p className='font-medium'>
                {order.orderDate
                  ? new Date(order.orderDate).toLocaleDateString()
                  : '-'}
              </p>
            </div>
          </div>

          {/* Payment Status */}
          <div className='flex items-start gap-3'>
            <div className='p-2 bg-gray-100 rounded-lg'>
              <Package className='h-5 w-5 text-gray-600' />
            </div>
            <div>
              <p className='text-sm text-gray-500'>
                {t(i18nKeyContainer.sales.orders.detail.paymentStatus)}
              </p>
              <PaymentStatusBadge status={order.paymentStatus} />
            </div>
          </div>

          {/* Notes */}
          <div className='flex items-start gap-3'>
            <div className='p-2 bg-gray-100 rounded-lg'>
              <FileText className='h-5 w-5 text-gray-600' />
            </div>
            <div>
              <p className='text-sm text-gray-500'>
                {t(i18nKeyContainer.sales.orders.detail.notes)}
              </p>
              <p className='font-medium'>
                {order.notes || (
                  <span className='text-gray-400 italic'>
                    {t(i18nKeyContainer.sales.orders.detail.noNotes)}
                  </span>
                )}
              </p>
            </div>
          </div>

          {/* Shipping Address */}
          <div className='flex items-start gap-3'>
            <div className='p-2 bg-gray-100 rounded-lg'>
              <MapPin className='h-5 w-5 text-gray-600' />
            </div>
            <div>
              <p className='text-sm text-gray-500'>
                {t(i18nKeyContainer.sales.orders.detail.shippingAddress)}
              </p>
              <p className='font-medium'>
                {order.shippingAddress || (
                  <span className='text-gray-400 italic'>
                    {t(i18nKeyContainer.sales.orders.detail.noShippingAddress)}
                  </span>
                )}
              </p>
            </div>
          </div>

          {/* Tracking Number */}
          <div className='flex items-start gap-3'>
            <div className='p-2 bg-gray-100 rounded-lg'>
              <Truck className='h-5 w-5 text-gray-600' />
            </div>
            <div>
              <p className='text-sm text-gray-500'>
                {t(i18nKeyContainer.sales.orders.detail.trackingNumber)}
              </p>
              <p className='font-medium'>
                {order.trackingNumber || (
                  <span className='text-gray-400 italic'>
                    {t(i18nKeyContainer.sales.orders.detail.noTrackingNumber)}
                  </span>
                )}
              </p>
            </div>
          </div>
        </div>
      </div>

      {/* Line Items */}
      <div className='bg-white rounded-lg shadow-sm border border-gray-200 p-6 mb-6'>
        <h3 className='text-lg font-medium text-gray-900 mb-4'>
          {t(i18nKeyContainer.sales.orders.detail.lineItems)}
        </h3>
        <div className='overflow-x-auto'>
          <table className='min-w-full divide-y divide-gray-200'>
            <thead className='bg-gray-50'>
              <tr>
                <th className='px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider'>
                  {t(i18nKeyContainer.sales.orders.items.product)}
                </th>
                <th className='px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider'>
                  {t(i18nKeyContainer.sales.orders.items.location)}
                </th>
                <th className='px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider'>
                  {t(i18nKeyContainer.sales.orders.items.quantity)}
                </th>
                <th className='px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider'>
                  {t(i18nKeyContainer.sales.orders.items.unitPrice)}
                </th>
                <th className='px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider'>
                  {t(i18nKeyContainer.sales.orders.items.subtotal)}
                </th>
              </tr>
            </thead>
            <tbody className='bg-white divide-y divide-gray-200'>
              {(order.items || []).map((item, index) => (
                <tr key={item.id || index}>
                  <td className='px-4 py-3'>
                    <div className='font-medium'>
                      {item.productName || item.product?.name}
                    </div>
                    {item.sku && (
                      <div className='text-sm text-gray-500'>{item.sku}</div>
                    )}
                  </td>
                  <td className='px-4 py-3 text-gray-600'>
                    {item.locationName || item.location?.name || '-'}
                  </td>
                  <td className='px-4 py-3'>{item.quantity}</td>
                  <td className='px-4 py-3'>
                    ${(item.unitPrice || 0).toFixed(2)}
                  </td>
                  <td className='px-4 py-3 font-medium'>
                    ${((item.quantity || 0) * (item.unitPrice || 0)).toFixed(2)}
                  </td>
                </tr>
              ))}
            </tbody>
            <tfoot className='bg-gray-50'>
              <tr>
                <td colSpan={4} className='px-4 py-3 text-right font-medium'>
                  {t(i18nKeyContainer.sales.orders.detail.orderTotal)}:
                </td>
                <td className='px-4 py-3 font-bold text-lg'>
                  ${orderTotal.toFixed(2)}
                </td>
              </tr>
            </tfoot>
          </table>
        </div>
      </div>
    </div>
  );
}

/**
 * Loading skeleton for order detail page
 */
function OrderDetailSkeleton() {
  return (
    <div className='max-w-4xl mx-auto animate-pulse'>
      <div className='flex items-center gap-4 mb-6'>
        <div className='h-8 w-20 bg-gray-200 rounded'></div>
        <div className='h-8 w-48 bg-gray-200 rounded'></div>
      </div>
      <div className='bg-white rounded-lg shadow-sm border border-gray-200 p-6 mb-6'>
        <div className='h-6 w-32 bg-gray-200 rounded mb-4'></div>
        <div className='grid grid-cols-2 gap-6'>
          {[1, 2, 3, 4].map(i => (
            <div key={i} className='flex gap-3'>
              <div className='h-10 w-10 bg-gray-200 rounded'></div>
              <div className='flex-1'>
                <div className='h-4 w-20 bg-gray-200 rounded mb-2'></div>
                <div className='h-5 w-32 bg-gray-200 rounded'></div>
              </div>
            </div>
          ))}
        </div>
      </div>
      <div className='bg-white rounded-lg shadow-sm border border-gray-200 p-6'>
        <div className='h-6 w-32 bg-gray-200 rounded mb-4'></div>
        <div className='space-y-3'>
          {[1, 2, 3].map(i => (
            <div key={i} className='h-12 bg-gray-100 rounded'></div>
          ))}
        </div>
      </div>
    </div>
  );
}
