import { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Check, Truck, Package, XCircle, RotateCcw, ArrowRight } from 'lucide-react';
import Button from '@components/Buttons/Button';
import ConfirmationDialog from '@components/ui/ConfirmationDialog';
import { useOrderTransition } from '@features/sales/hooks/useOrders';
import { STATUS_TRANSITIONS, ACTION_CONFIG } from '@features/sales/utils/orderConstants';
import i18nKeyContainer from '@shared/lib/i18n/keyContainer';

/**
 * OrderActionBar Component
 *
 * Renders status transition buttons based on current order status.
 * Handles confirmation dialogs and tracking number input for shipping.
 *
 * @param {Object} props - Component props
 * @param {Object} props.order - Full order object with id and status
 * @param {function} props.onTransitionSuccess - Optional callback after successful transition
 */
const OrderActionBar = ({ order, onTransitionSuccess }) => {
  const { t } = useTranslation();
  const transition = useOrderTransition();
  const [confirmAction, setConfirmAction] = useState(null);
  const [trackingNumber, setTrackingNumber] = useState('');
  const [showShipDialog, setShowShipDialog] = useState(false);

  if (!order?.status) return null;

  const availableActions = STATUS_TRANSITIONS[order.status] || [];

  if (availableActions.length === 0) return null;

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
        return ArrowRight;
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
      ship: {
        title: t(i18nKeyContainer.sales.orders.confirmDialog.shipTitle),
        message: t(i18nKeyContainer.sales.orders.confirmDialog.shipMessage),
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

  const handleActionClick = action => {
    if (action === 'ship') {
      setShowShipDialog(true);
    } else {
      setConfirmAction(action);
    }
  };

  const handleConfirm = async () => {
    if (!confirmAction) return;

    try {
      await transition.mutateAsync({
        orderId: order.id,
        action: confirmAction,
      });
      setConfirmAction(null);
      onTransitionSuccess?.();
    } catch (error) {
      // Error handling is done in the hook
    }
  };

  const handleShipConfirm = async () => {
    try {
      await transition.mutateAsync({
        orderId: order.id,
        action: 'ship',
        payload: trackingNumber ? { trackingNumber } : {},
      });
      setShowShipDialog(false);
      setTrackingNumber('');
      onTransitionSuccess?.();
    } catch (error) {
      // Error handling is done in the hook
    }
  };

  const dialogConfig = confirmAction ? getDialogConfig(confirmAction) : {};

  return (
    <>
      <div className="flex flex-wrap gap-2">
        {availableActions.map(action => {
          const config = ACTION_CONFIG[action];
          const Icon = getActionIcon(action);
          const isDestructive = config.variant === 'destructive';

          return (
            <Button
              key={action}
              variant={isDestructive ? 'danger' : 'secondary'}
              size="sm"
              LeftIcon={Icon}
              onClick={() => handleActionClick(action)}
              disabled={transition.isPending}
              loading={transition.isPending && transition.variables?.action === action}
            >
              {t(config.labelKey)}
            </Button>
          );
        })}
      </div>

      {/* Standard confirmation dialog */}
      {confirmAction && confirmAction !== 'ship' && (
        <ConfirmationDialog
          isOpen={!!confirmAction}
          onClose={() => setConfirmAction(null)}
          onConfirm={handleConfirm}
          type={dialogConfig.type}
          title={dialogConfig.title}
          message={dialogConfig.message}
          confirmText={t(i18nKeyContainer.sales.shared.confirm)}
          loading={transition.isPending}
        />
      )}

      {/* Ship dialog with tracking number input */}
      {showShipDialog && (
        <div className="fixed inset-0 z-[9999] flex items-center justify-center bg-black/50 backdrop-blur-sm">
          <div className="relative w-full max-w-md mx-4 bg-white rounded-xl shadow-2xl">
            <div className="bg-purple-50 border-b border-purple-200 rounded-t-xl p-6">
              <div className="flex items-start gap-4">
                <div className="bg-purple-100 rounded-full p-3 flex-shrink-0">
                  <Package className="w-6 h-6 text-purple-600" />
                </div>
                <div className="flex-1 min-w-0">
                  <h2 className="text-xl font-semibold text-gray-900 mb-1">
                    {t(i18nKeyContainer.sales.orders.confirmDialog.shipTitle)}
                  </h2>
                </div>
              </div>
            </div>

            <div className="p-6 space-y-4">
              <p className="text-sm text-gray-600">
                {t(i18nKeyContainer.sales.orders.confirmDialog.shipMessage)}
              </p>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  {t(i18nKeyContainer.sales.orders.confirmDialog.trackingNumber)}{' '}
                  <span className="text-gray-400">
                    {t(i18nKeyContainer.sales.shared.optional)}
                  </span>
                </label>
                <input
                  type="text"
                  value={trackingNumber}
                  onChange={e => setTrackingNumber(e.target.value)}
                  placeholder={t(
                    i18nKeyContainer.sales.orders.confirmDialog.trackingNumberPlaceholder
                  )}
                  className="block w-full rounded-md border-gray-300 shadow-sm focus:border-purple-500 focus:ring-purple-500 sm:text-sm"
                />
              </div>
            </div>

            <div className="flex items-center justify-end gap-3 px-6 py-4 bg-gray-50 rounded-b-xl border-t border-gray-200">
              <button
                onClick={() => {
                  setShowShipDialog(false);
                  setTrackingNumber('');
                }}
                disabled={transition.isPending}
                className="px-4 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded-lg hover:bg-gray-50 transition-colors disabled:opacity-50"
                type="button"
              >
                {t(i18nKeyContainer.sales.shared.cancel)}
              </button>
              <button
                onClick={handleShipConfirm}
                disabled={transition.isPending}
                className="px-4 py-2 text-sm font-medium text-white bg-purple-600 rounded-lg hover:bg-purple-700 transition-colors disabled:opacity-50 flex items-center gap-2"
                type="button"
              >
                {transition.isPending && (
                  <svg
                    className="animate-spin h-4 w-4 text-white"
                    xmlns="http://www.w3.org/2000/svg"
                    fill="none"
                    viewBox="0 0 24 24"
                  >
                    <circle
                      className="opacity-25"
                      cx="12"
                      cy="12"
                      r="10"
                      stroke="currentColor"
                      strokeWidth="4"
                    />
                    <path
                      className="opacity-75"
                      fill="currentColor"
                      d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"
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
  );
};

export default OrderActionBar;
