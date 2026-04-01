import { useTranslation } from 'react-i18next';
import { cn } from '@shared/lib/utils';
import { PAYMENT_STATUS_BADGE_CLASSES, PAYMENT_STATUS } from '@features/sales/utils/orderConstants';
import i18nKeyContainer from '@shared/lib/i18n/keyContainer';

/**
 * PaymentStatusBadge Component
 *
 * Displays payment status with consistent colors across the application.
 *
 * @param {Object} props - Component props
 * @param {string} props.status - Payment status value (Unpaid, PartiallyPaid, Paid)
 * @param {string} props.className - Optional additional CSS classes
 */
const PaymentStatusBadge = ({ status, className }) => {
  const { t } = useTranslation();

  const badgeClasses =
    PAYMENT_STATUS_BADGE_CLASSES[status] || PAYMENT_STATUS_BADGE_CLASSES[PAYMENT_STATUS.Unpaid];
  const statusKey =
    i18nKeyContainer.sales.orders.paymentStatus[status?.toLowerCase()] || status;

  return (
    <span
      className={cn(
        'inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium border',
        badgeClasses,
        className
      )}
    >
      {t(statusKey)}
    </span>
  );
};

export default PaymentStatusBadge;
