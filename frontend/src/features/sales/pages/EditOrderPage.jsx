import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { ArrowLeft, AlertCircle } from 'lucide-react';
import PageHeader from '@components/ui/PageHeader';
import Button from '@components/Buttons/Button';
import OrderItemsForm from '@features/sales/components/OrderItemsForm';
import OrderActionBar from '@features/sales/components/OrderActionBar';
import { useOrder, useUpdateOrder } from '@features/sales/hooks/useOrders';
import { ORDER_STATUS } from '@features/sales/utils/orderConstants';
import { useToast } from '@shared/context/ToastContext';
import i18nKeyContainer from '@shared/lib/i18n/keyContainer';

const PAYMENT_STATUS_OPTIONS = [
  {
    value: 1,
    labelKey: i18nKeyContainer.sales.orders.paymentStatus.unpaid,
  },
  {
    value: 2,
    labelKey: i18nKeyContainer.sales.orders.paymentStatus.partiallypaid,
  },
  {
    value: 3,
    labelKey: i18nKeyContainer.sales.orders.paymentStatus.paid,
  },
];

const DEFAULT_PAYMENT_STATUS = PAYMENT_STATUS_OPTIONS[0].value;

const PAYMENT_STATUS_VALUE_MAP = {
  Unpaid: 1,
  PartiallyPaid: 2,
  Paid: 3,
};

/**
 * EditOrderPage Component
 *
 * Edit page for modifying order items and notes.
 * Only accessible for orders with Pending status.
 *
 * @route /sales-orders/:id/edit
 */
export default function EditOrderPage() {
  const { t } = useTranslation();
  const { id } = useParams();
  const navigate = useNavigate();
  const { showError } = useToast();
  const updateOrder = useUpdateOrder();

  const { data: orderResponse, isLoading } = useOrder(id);

  const [items, setItems] = useState([]);
  const [notes, setNotes] = useState('');
  const [shippingAddress, setShippingAddress] = useState('');
  const [paymentStatus, setPaymentStatus] = useState(
    String(DEFAULT_PAYMENT_STATUS)
  );
  const [initializedOrderId, setInitializedOrderId] = useState(null);

  const order = orderResponse?.success ? orderResponse.data : null;

  // Redirect if not pending
  useEffect(() => {
    if (order && order.status !== ORDER_STATUS.Pending) {
      navigate(`/sales-orders/${id}`);
    }
  }, [order, id, navigate]);

  // Initialize form with order data
  useEffect(() => {
    if (order?.id && initializedOrderId !== order.id) {
      setItems(
        (order.items || []).map((item, index) => ({
          id: item.id,
          productId: item.productId?.toString() || '',
          locationId: item.locationId?.toString() || '',
          productName: item.productName || item.product?.name || '',
          locationName: item.locationName || item.location?.name || '',
          quantity: item.quantity || 1,
          unitPrice: item.unitPrice || 0,
          _tempId: item.id
            ? `order-item-${item.id}`
            : `order-item-${order.id}-${index}`,
        }))
      );
      setNotes(order.notes || order.description || '');
      setShippingAddress(order.shippingAddress || '');
      setPaymentStatus(
        String(
          PAYMENT_STATUS_VALUE_MAP[order.paymentStatus] ||
            DEFAULT_PAYMENT_STATUS
        )
      );
      setInitializedOrderId(order.id);
    }
  }, [order, initializedOrderId]);

  const validateForm = () => {
    if (items.length === 0) {
      showError(
        t(i18nKeyContainer.sales.orders.toasts.validationError),
        t(i18nKeyContainer.sales.orders.toasts.atLeastOneItem)
      );
      return false;
    }

    for (const item of items) {
      if (!item.productId || !item.locationId || !item.quantity) {
        showError(
          t(i18nKeyContainer.sales.orders.toasts.validationError),
          t(i18nKeyContainer.sales.orders.toasts.atLeastOneItem)
        );
        return false;
      }
    }

    return true;
  };

  const handleSubmit = async e => {
    e.preventDefault();

    if (!validateForm()) return;

    const parsedPaymentStatus = Number(paymentStatus);
    const normalizedPaymentStatus = PAYMENT_STATUS_OPTIONS.some(
      option => option.value === parsedPaymentStatus
    )
      ? parsedPaymentStatus
      : DEFAULT_PAYMENT_STATUS;

    const orderData = {
      customerId: order.isWalkIn ? null : order.customerId || null,
      description: notes || null,
      shippingAddress: shippingAddress || null,
      paymentStatus: normalizedPaymentStatus,
      items: items.map(item => ({
        productId: parseInt(item.productId),
        locationId: parseInt(item.locationId),
        quantity: parseFloat(item.quantity),
      })),
    };

    try {
      const result = await updateOrder.mutateAsync({ id, data: orderData });
      if (result.success) {
        navigate(`/sales-orders/${id}`);
      }
    } catch {
      // Error handling is done in the hook
    }
  };

  if (isLoading) {
    return <EditOrderSkeleton />;
  }

  if (!order) {
    return (
      <div className='w-full max-w-none px-2 md:px-4'>
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
          <h3 className='text-lg font-medium text-gray-900'>
            {t(i18nKeyContainer.sales.orders.detail.notFound)}
          </h3>
        </div>
      </div>
    );
  }

  return (
    <div className='w-full max-w-none px-2 md:px-4'>
      {/* Header */}
      <div className='flex items-center gap-4 mb-6'>
        <Button
          variant='ghost'
          size='sm'
          LeftIcon={ArrowLeft}
          onClick={() => navigate(`/sales-orders/${id}`)}
        >
          {t(i18nKeyContainer.sales.shared.back)}
        </Button>
        <PageHeader
          title={t(i18nKeyContainer.sales.orders.edit.title, {
            orderNumber: order.orderNumber || order.id,
          })}
          description=''
        />
      </div>

      {/* Info Banner */}
      <div className='bg-blue-50 border border-blue-200 rounded-lg p-4 mb-6 flex items-center gap-3'>
        <AlertCircle className='h-5 w-5 text-blue-600 flex-shrink-0' />
        <p className='text-sm text-blue-800'>
          {t(i18nKeyContainer.sales.orders.edit.onlyPendingCanEdit)}
        </p>
      </div>

      <form onSubmit={handleSubmit} className='space-y-6'>
        <div className='bg-white rounded-lg shadow-sm border border-gray-200 p-4'>
          <OrderActionBar
            order={order}
            mode='compact'
            onTransitionSuccess={() => navigate(`/sales-orders/${id}`)}
          />
        </div>

        {/* Customer Info (Read-only) */}
        <div className='bg-white rounded-lg shadow-sm border border-gray-200 p-6'>
          <h3 className='font-medium text-gray-900 mb-2'>
            {t(i18nKeyContainer.sales.orders.detail.customer)}
          </h3>
          <p className='text-gray-600'>
            {order.customerName || (
              <span className='italic'>
                {t(i18nKeyContainer.sales.orders.table.walkIn)}
              </span>
            )}
          </p>
        </div>

        {/* Payment Status */}
        <div className='bg-white rounded-lg shadow-sm border border-gray-200 p-6'>
          <label className='block text-sm font-medium text-gray-700 mb-2'>
            {t(i18nKeyContainer.sales.orders.detail.paymentStatus)}
          </label>
          <select
            value={paymentStatus}
            onChange={e => setPaymentStatus(e.target.value)}
            className='block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm'
          >
            {PAYMENT_STATUS_OPTIONS.map(option => (
              <option key={option.value} value={option.value}>
                {t(option.labelKey)}
              </option>
            ))}
          </select>
        </div>

        {/* Order Items */}
        <div className='bg-white rounded-lg shadow-sm border border-gray-200 p-3 md:p-4 lg:p-5'>
          <OrderItemsForm value={items} onChange={setItems} />
        </div>

        {/* Notes */}
        <div className='bg-white rounded-lg shadow-sm border border-gray-200 p-6'>
          <label className='block text-sm font-medium text-gray-700 mb-2'>
            {t(i18nKeyContainer.sales.orders.create.notes)}{' '}
            <span className='text-gray-400'>
              {t(i18nKeyContainer.sales.shared.optional)}
            </span>
          </label>
          <textarea
            value={notes}
            onChange={e => setNotes(e.target.value)}
            rows={3}
            placeholder={t(
              i18nKeyContainer.sales.orders.create.notesPlaceholder
            )}
            className='block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm'
          />
        </div>

        {/* Shipping Address */}
        <div className='bg-white rounded-lg shadow-sm border border-gray-200 p-6'>
          <label className='block text-sm font-medium text-gray-700 mb-2'>
            {t(i18nKeyContainer.sales.orders.detail.shippingAddress)}{' '}
            <span className='text-gray-400'>
              {t(i18nKeyContainer.sales.shared.optional)}
            </span>
          </label>
          <textarea
            value={shippingAddress}
            onChange={e => setShippingAddress(e.target.value)}
            rows={2}
            placeholder={t(
              i18nKeyContainer.sales.orders.detail.noShippingAddress
            )}
            className='block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm'
          />
        </div>

        {/* Actions */}
        <div className='flex justify-end gap-3'>
          <Button
            type='button'
            variant='secondary'
            onClick={() => navigate(`/sales-orders/${id}`)}
            disabled={updateOrder.isPending}
          >
            {t(i18nKeyContainer.sales.shared.cancel)}
          </Button>
          <Button
            type='submit'
            disabled={updateOrder.isPending || items.length === 0}
            loading={updateOrder.isPending}
          >
            {updateOrder.isPending
              ? t(i18nKeyContainer.sales.orders.edit.saving)
              : t(i18nKeyContainer.sales.orders.edit.saveChanges)}
          </Button>
        </div>
      </form>
    </div>
  );
}

/**
 * Loading skeleton for edit page
 */
function EditOrderSkeleton() {
  return (
    <div className='w-full max-w-none px-2 md:px-4 animate-pulse'>
      <div className='flex items-center gap-4 mb-6'>
        <div className='h-8 w-20 bg-gray-200 rounded'></div>
        <div className='h-8 w-48 bg-gray-200 rounded'></div>
      </div>
      <div className='bg-blue-50 rounded-lg p-4 mb-6'>
        <div className='h-5 w-full bg-blue-100 rounded'></div>
      </div>
      <div className='bg-white rounded-lg shadow-sm border border-gray-200 p-6 mb-6'>
        <div className='h-6 w-24 bg-gray-200 rounded mb-2'></div>
        <div className='h-5 w-32 bg-gray-200 rounded'></div>
      </div>
      <div className='bg-white rounded-lg shadow-sm border border-gray-200 p-6'>
        <div className='h-6 w-32 bg-gray-200 rounded mb-4'></div>
        <div className='space-y-3'>
          {[1, 2].map(i => (
            <div key={i} className='h-12 bg-gray-100 rounded'></div>
          ))}
        </div>
      </div>
    </div>
  );
}
