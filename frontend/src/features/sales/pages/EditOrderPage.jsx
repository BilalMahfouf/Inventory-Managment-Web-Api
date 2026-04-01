import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { ArrowLeft, AlertCircle } from 'lucide-react';
import PageHeader from '@components/ui/PageHeader';
import Button from '@components/Buttons/Button';
import OrderItemsForm from '@features/sales/components/OrderItemsForm';
import { useOrder, useUpdateOrder } from '@features/sales/hooks/useOrders';
import { ORDER_STATUS } from '@features/sales/utils/orderConstants';
import { useToast } from '@shared/context/ToastContext';
import i18nKeyContainer from '@shared/lib/i18n/keyContainer';

/**
 * EditOrderPage Component
 *
 * Edit page for modifying order items and notes.
 * Only accessible for orders with Pending status.
 *
 * @route /orders/:id/edit
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
  const [initialized, setInitialized] = useState(false);

  const order = orderResponse?.success ? orderResponse.data : null;

  // Redirect if not pending
  useEffect(() => {
    if (order && order.status !== ORDER_STATUS.Pending) {
      navigate(`/orders/${id}`);
    }
  }, [order, id, navigate]);

  // Initialize form with order data
  useEffect(() => {
    if (order && !initialized) {
      setItems(
        (order.items || []).map(item => ({
          id: item.id,
          productId: item.productId?.toString() || '',
          locationId: item.locationId?.toString() || '',
          quantity: item.quantity || 1,
          unitPrice: item.unitPrice || 0,
          _tempId: item.id || Date.now(),
        }))
      );
      setNotes(order.notes || '');
      setInitialized(true);
    }
  }, [order, initialized]);

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

    const orderData = {
      notes: notes || null,
      items: items.map(item => ({
        id: item.id || undefined,
        productId: parseInt(item.productId),
        locationId: parseInt(item.locationId),
        quantity: parseFloat(item.quantity),
        unitPrice: parseFloat(item.unitPrice),
      })),
    };

    try {
      const result = await updateOrder.mutateAsync({ id, data: orderData });
      if (result.success) {
        navigate(`/orders/${id}`);
      }
    } catch (error) {
      // Error handling is done in the hook
    }
  };

  if (isLoading) {
    return <EditOrderSkeleton />;
  }

  if (!order) {
    return (
      <div className="max-w-4xl mx-auto">
        <div className="flex items-center gap-4 mb-6">
          <Button
            variant="ghost"
            size="sm"
            LeftIcon={ArrowLeft}
            onClick={() => navigate('/orders')}
          >
            {t(i18nKeyContainer.sales.shared.back)}
          </Button>
        </div>
        <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-12 text-center">
          <h3 className="text-lg font-medium text-gray-900">
            {t(i18nKeyContainer.sales.orders.detail.notFound)}
          </h3>
        </div>
      </div>
    );
  }

  return (
    <div className="max-w-4xl mx-auto">
      {/* Header */}
      <div className="flex items-center gap-4 mb-6">
        <Button
          variant="ghost"
          size="sm"
          LeftIcon={ArrowLeft}
          onClick={() => navigate(`/orders/${id}`)}
        >
          {t(i18nKeyContainer.sales.shared.back)}
        </Button>
        <PageHeader
          title={t(i18nKeyContainer.sales.orders.edit.title, {
            orderNumber: order.orderNumber || order.id,
          })}
          description=""
        />
      </div>

      {/* Info Banner */}
      <div className="bg-blue-50 border border-blue-200 rounded-lg p-4 mb-6 flex items-center gap-3">
        <AlertCircle className="h-5 w-5 text-blue-600 flex-shrink-0" />
        <p className="text-sm text-blue-800">
          {t(i18nKeyContainer.sales.orders.edit.onlyPendingCanEdit)}
        </p>
      </div>

      <form onSubmit={handleSubmit} className="space-y-6">
        {/* Customer Info (Read-only) */}
        <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
          <h3 className="font-medium text-gray-900 mb-2">
            {t(i18nKeyContainer.sales.orders.detail.customer)}
          </h3>
          <p className="text-gray-600">
            {order.customerName || (
              <span className="italic">
                {t(i18nKeyContainer.sales.orders.table.walkIn)}
              </span>
            )}
          </p>
        </div>

        {/* Order Items */}
        <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
          <OrderItemsForm value={items} onChange={setItems} />
        </div>

        {/* Notes */}
        <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
          <label className="block text-sm font-medium text-gray-700 mb-2">
            {t(i18nKeyContainer.sales.orders.create.notes)}{' '}
            <span className="text-gray-400">
              {t(i18nKeyContainer.sales.shared.optional)}
            </span>
          </label>
          <textarea
            value={notes}
            onChange={e => setNotes(e.target.value)}
            rows={3}
            placeholder={t(i18nKeyContainer.sales.orders.create.notesPlaceholder)}
            className="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm"
          />
        </div>

        {/* Actions */}
        <div className="flex justify-end gap-3">
          <Button
            type="button"
            variant="secondary"
            onClick={() => navigate(`/orders/${id}`)}
            disabled={updateOrder.isPending}
          >
            {t(i18nKeyContainer.sales.shared.cancel)}
          </Button>
          <Button
            type="submit"
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
    <div className="max-w-4xl mx-auto animate-pulse">
      <div className="flex items-center gap-4 mb-6">
        <div className="h-8 w-20 bg-gray-200 rounded"></div>
        <div className="h-8 w-48 bg-gray-200 rounded"></div>
      </div>
      <div className="bg-blue-50 rounded-lg p-4 mb-6">
        <div className="h-5 w-full bg-blue-100 rounded"></div>
      </div>
      <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6 mb-6">
        <div className="h-6 w-24 bg-gray-200 rounded mb-2"></div>
        <div className="h-5 w-32 bg-gray-200 rounded"></div>
      </div>
      <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
        <div className="h-6 w-32 bg-gray-200 rounded mb-4"></div>
        <div className="space-y-3">
          {[1, 2].map(i => (
            <div key={i} className="h-12 bg-gray-100 rounded"></div>
          ))}
        </div>
      </div>
    </div>
  );
}
