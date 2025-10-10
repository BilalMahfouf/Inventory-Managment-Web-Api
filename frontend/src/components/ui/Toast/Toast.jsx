import { useEffect } from 'react';
import { X, CheckCircle, XCircle, AlertCircle, Info } from 'lucide-react';

/**
 * Toast Component
 *
 * A generic notification component that displays success, error, warning, or info messages.
 * Auto-dismisses after a specified duration and supports manual closing.
 * Uses only Tailwind CSS classes - no custom CSS modifications needed.
 *
 * @param {Object} props - Component props
 * @param {string} props.type - Toast type: 'success', 'error', 'warning', 'info'
 * @param {string} props.title - Main heading text
 * @param {string} props.message - Detailed message (optional)
 * @param {number} props.duration - Auto-dismiss duration in ms (default: 5000, null = no auto-dismiss)
 * @param {function} props.onClose - Callback when toast is closed
 * @param {boolean} props.showClose - Show close button (default: true)
 *
 * @example
 * ```jsx
 * <Toast
 *   type="success"
 *   title="Product Created Successfully"
 *   message="The product has been added to your inventory."
 *   onClose={() => console.log('closed')}
 * />
 * ```
 */
const Toast = ({
  type = 'info',
  title,
  message,
  duration = 5000,
  onClose,
  showClose = true,
}) => {
  useEffect(() => {
    if (duration && onClose) {
      const timer = setTimeout(() => {
        onClose();
      }, duration);

      return () => clearTimeout(timer);
    }
  }, [duration, onClose]);

  // Configuration for each toast type
  const config = {
    success: {
      icon: CheckCircle,
      bgColor: 'bg-green-50',
      borderColor: 'border-green-500',
      iconColor: 'text-green-600',
      titleColor: 'text-green-900',
      messageColor: 'text-green-700',
    },
    error: {
      icon: XCircle,
      bgColor: 'bg-red-50',
      borderColor: 'border-red-500',
      iconColor: 'text-red-600',
      titleColor: 'text-red-900',
      messageColor: 'text-red-700',
    },
    warning: {
      icon: AlertCircle,
      bgColor: 'bg-orange-50',
      borderColor: 'border-orange-500',
      iconColor: 'text-orange-600',
      titleColor: 'text-orange-900',
      messageColor: 'text-orange-700',
    },
    info: {
      icon: Info,
      bgColor: 'bg-blue-50',
      borderColor: 'border-blue-500',
      iconColor: 'text-blue-600',
      titleColor: 'text-blue-900',
      messageColor: 'text-blue-700',
    },
  };

  const currentConfig = config[type] || config.info;
  const Icon = currentConfig.icon;

  return (
    <div
      className={`
        ${currentConfig.bgColor} 
        ${currentConfig.borderColor}
        border-l-4 rounded-lg shadow-lg p-4 mb-4 min-w-[320px] max-w-md
        transform transition-all duration-300 ease-out
        animate-in slide-in-from-right-full
      `}
      role='alert'
      aria-live='polite'
    >
      <div className='flex items-start gap-3'>
        {/* Icon */}
        <div className='flex-shrink-0 mt-0.5'>
          <Icon className={`w-5 h-5 ${currentConfig.iconColor}`} />
        </div>

        {/* Content */}
        <div className='flex-1 min-w-0'>
          <h3
            className={`text-sm font-semibold ${currentConfig.titleColor} mb-0.5`}
          >
            {title}
          </h3>
          {message && (
            <p className={`text-sm ${currentConfig.messageColor}`}>{message}</p>
          )}
        </div>

        {/* Close Button */}
        {showClose && onClose && (
          <button
            onClick={onClose}
            className={`
              flex-shrink-0 ${currentConfig.iconColor} 
              hover:opacity-70 transition-opacity duration-150
              focus:outline-none focus:ring-2 focus:ring-offset-1 focus:ring-current rounded cursor-pointer
            `}
            aria-label='Close notification'
          >
            <X className='w-5 h-5' />
          </button>
        )}
      </div>
    </div>
  );
};

export default Toast;
