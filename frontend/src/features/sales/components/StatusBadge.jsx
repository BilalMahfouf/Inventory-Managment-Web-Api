import { useTranslation } from 'react-i18next';
import { cn } from '@shared/lib/utils';
import { STATUS_BADGE_CLASSES, ORDER_STATUS } from '@features/sales/utils/orderConstants';
import i18nKeyContainer from '@shared/lib/i18n/keyContainer';

/**
 * StatusBadge Component
 *
 * Displays order status with consistent colors across the application.
 *
 * @param {Object} props - Component props
 * @param {string} props.status - Order status value (Pending, Confirmed, etc.)
 * @param {string} props.className - Optional additional CSS classes
 */
const StatusBadge = ({ status, className }) => {
  const { t } = useTranslation();

  const badgeClasses = STATUS_BADGE_CLASSES[status] || STATUS_BADGE_CLASSES[ORDER_STATUS.Pending];
  const statusKey = i18nKeyContainer.sales.orders.status[status?.toLowerCase()] || status;

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

export default StatusBadge;
