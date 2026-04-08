import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { useTranslation } from 'react-i18next';
import { ArrowLeft, ShoppingCart, User } from 'lucide-react';
import PageHeader from '@components/ui/PageHeader';
import Button from '@components/Buttons/Button';
import OrderItemsForm from '@features/sales/components/OrderItemsForm';
import { useCreateOrder } from '@features/sales/hooks/useOrders';
import { getCustomers } from '@features/customers/services/customerApi';
import { useToast } from '@shared/context/ToastContext';
import { queryKeys } from '@shared/lib/queryKeys';
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

/**
 * CreateOrderPage Component
 *
 * Form page for creating new sales orders.
 * Supports walk-in sales and customer orders.
 *
 * @route /orders/new
 */
export default function CreateOrderPage() {
  const { t } = useTranslation();
  const navigate = useNavigate();
  const { showError } = useToast();
  const createOrder = useCreateOrder();

  const [isWalkIn, setIsWalkIn] = useState(false);
  const [customerId, setCustomerId] = useState('');
  const [paymentStatus, setPaymentStatus] = useState(
    String(DEFAULT_PAYMENT_STATUS)
  );
  const [items, setItems] = useState([]);
  const [notes, setNotes] = useState('');

  // Fetch customers for dropdown
  const {
    data: customersResponse,
    isLoading: customersLoading,
    isError: customersQueryError,
    error: customersQueryErrorDetails,
  } = useQuery({
    queryKey: queryKeys.customers.table({ page: 1, pageSize: 100 }),
    queryFn: () => getCustomers({ page: 1, pageSize: 100 }),
  });

  const customersPayload = customersResponse?.success
    ? customersResponse?.data
    : null;
  const customers = Array.isArray(customersPayload?.item)
    ? customersPayload.item
    : Array.isArray(customersPayload?.items)
      ? customersPayload.items
      : [];

  const customersErrorMessage = customersQueryError
    ? customersQueryErrorDetails?.message || 'Failed to load customers.'
    : customersResponse?.success === false
      ? customersResponse?.error || 'Failed to load customers.'
      : null;

  const calculateTotal = () => {
    return items.reduce((sum, item) => {
      return (
        sum +
        (parseFloat(item.quantity) || 0) * (parseFloat(item.unitPrice) || 0)
      );
    }, 0);
  };

  const validateForm = () => {
    if (items.length === 0) {
      showError(
        t(i18nKeyContainer.sales.orders.toasts.validationError),
        t(i18nKeyContainer.sales.orders.toasts.atLeastOneItem)
      );
      return false;
    }

    // Check all items have required fields
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
      customerId: isWalkIn ? null : customerId || null,
      isWalkIn,
      paymentStatus: normalizedPaymentStatus,
      notes: notes || null,
      items: items.map(item => ({
        productId: parseInt(item.productId),
        locationId: parseInt(item.locationId),
        quantity: parseFloat(item.quantity),
        unitPrice: parseFloat(item.unitPrice),
      })),
    };

    try {
      const result = await createOrder.mutateAsync(orderData);
      if (result.success) {
        navigate(`/orders/${result.data?.id || ''}`);
      }
    } catch {
      // Error handling is done in the hook
    }
  };

  return (
    <div className='w-full max-w-none px-2 md:px-4'>
      {/* Header */}
      <div className='flex items-center gap-4 mb-6'>
        <Button
          variant='ghost'
          size='sm'
          LeftIcon={ArrowLeft}
          onClick={() => navigate('/orders')}
        >
          {t(i18nKeyContainer.sales.shared.back)}
        </Button>
        <PageHeader
          title={t(i18nKeyContainer.sales.orders.create.title)}
          description=''
        />
      </div>

      <form onSubmit={handleSubmit} className='space-y-6'>
        {/* Walk-in Toggle */}
        <div className='bg-white rounded-lg shadow-sm border border-gray-200 p-6'>
          <div className='flex items-center justify-between'>
            <div className='flex items-center gap-3'>
              <div className='p-2 bg-blue-100 rounded-lg'>
                <ShoppingCart className='h-5 w-5 text-blue-600' />
              </div>
              <div>
                <h3 className='font-medium text-gray-900'>
                  {t(i18nKeyContainer.sales.orders.create.walkInToggle)}
                </h3>
                <p className='text-sm text-gray-500'>
                  {t(i18nKeyContainer.sales.orders.create.walkInDescription)}
                </p>
              </div>
            </div>
            <label className='relative inline-flex items-center cursor-pointer'>
              <input
                type='checkbox'
                checked={isWalkIn}
                onChange={e => {
                  setIsWalkIn(e.target.checked);
                  if (e.target.checked) {
                    setCustomerId('');
                  }
                }}
                className='sr-only peer'
              />
              <div className="w-11 h-6 bg-gray-200 peer-focus:outline-none peer-focus:ring-4 peer-focus:ring-blue-300 rounded-full peer peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[2px] after:left-[2px] after:bg-white after:border-gray-300 after:border after:rounded-full after:h-5 after:w-5 after:transition-all peer-checked:bg-blue-600"></div>
            </label>
          </div>
        </div>

        {/* Customer Selection (hidden for walk-in) */}
        {!isWalkIn && (
          <div className='bg-white rounded-lg shadow-sm border border-gray-200 p-6'>
            <div className='flex items-center gap-3 mb-4'>
              <div className='p-2 bg-gray-100 rounded-lg'>
                <User className='h-5 w-5 text-gray-600' />
              </div>
              <h3 className='font-medium text-gray-900'>
                {t(i18nKeyContainer.sales.orders.create.selectCustomer)}
              </h3>
            </div>
            <select
              value={customerId}
              onChange={e => setCustomerId(e.target.value)}
              disabled={customersLoading || !!customersErrorMessage}
              className='block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm'
            >
              <option value=''>
                {customersLoading
                  ? 'Loading customers...'
                  : customersErrorMessage
                    ? 'Failed to load customers'
                    : customers.length === 0
                      ? 'No customers found'
                      : t(
                          i18nKeyContainer.sales.orders.create
                            .selectCustomerPlaceholder
                        )}
              </option>
              {customers.map(customer => (
                <option key={customer.id} value={customer.id}>
                  {customer.name} {customer.email ? `(${customer.email})` : ''}
                </option>
              ))}
            </select>
            {customersErrorMessage && (
              <p className='mt-2 text-sm text-red-600'>
                {customersErrorMessage}
              </p>
            )}
          </div>
        )}

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

        {/* Order Summary */}
        <div className='bg-white rounded-lg shadow-sm border border-gray-200 p-6'>
          <h3 className='font-medium text-gray-900 mb-4'>
            {t(i18nKeyContainer.sales.orders.create.orderSummary)}
          </h3>
          <div className='flex justify-between items-center text-lg font-bold'>
            <span>{t(i18nKeyContainer.sales.orders.create.total)}:</span>
            <span className='text-2xl'>${calculateTotal().toFixed(2)}</span>
          </div>
        </div>

        {/* Actions */}
        <div className='flex justify-end gap-3'>
          <Button
            type='button'
            variant='secondary'
            onClick={() => navigate('/orders')}
            disabled={createOrder.isPending}
          >
            {t(i18nKeyContainer.sales.shared.cancel)}
          </Button>
          <Button
            type='submit'
            disabled={createOrder.isPending || items.length === 0}
            loading={createOrder.isPending}
          >
            {createOrder.isPending
              ? t(i18nKeyContainer.sales.orders.create.creating)
              : isWalkIn
                ? t(i18nKeyContainer.sales.orders.create.completeSale)
                : t(i18nKeyContainer.sales.orders.create.createOrder)}
          </Button>
        </div>
      </form>
    </div>
  );
}
