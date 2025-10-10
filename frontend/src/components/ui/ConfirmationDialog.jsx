import React from 'react';
import { Trash2, AlertCircle, CheckCircle } from 'lucide-react';

/**
 * ConfirmationDialog Component
 *
 * A generic, reusable confirmation dialog for Create, Update, and Delete operations.
 * Supports different visual themes based on the operation type.
 *
 * @param {Object} props - Component props
 * @param {boolean} props.isOpen - Controls dialog visibility
 * @param {function} props.onClose - Callback when dialog is closed (Cancel clicked or backdrop clicked)
 * @param {function} props.onConfirm - Callback when confirmation button is clicked
 * @param {string} props.type - Dialog type: 'delete', 'update', 'create' (default: 'delete')
 * @param {string} props.title - Dialog title
 * @param {string} props.message - Confirmation message
 * @param {string} props.itemName - Name of the item being acted upon (optional)
 * @param {string} props.confirmText - Text for confirm button (default based on type)
 * @param {string} props.cancelText - Text for cancel button (default: 'Cancel')
 * @param {boolean} props.loading - Show loading state on confirm button (default: false)
 *
 * @example
 * // Delete confirmation
 * <ConfirmationDialog
 *   isOpen={showDialog}
 *   onClose={() => setShowDialog(false)}
 *   onConfirm={handleDelete}
 *   type="delete"
 *   title="Delete Product"
 *   itemName="iPhone 15 Pro"
 *   message="This action cannot be undone and will permanently remove this product from the system."
 * />
 *
 * @example
 * // Update confirmation
 * <ConfirmationDialog
 *   isOpen={showDialog}
 *   onClose={() => setShowDialog(false)}
 *   onConfirm={handleUpdate}
 *   type="update"
 *   title="Update Product"
 *   message="Are you sure you want to update this product?"
 * />
 *
 * @example
 * // Create confirmation
 * <ConfirmationDialog
 *   isOpen={showDialog}
 *   onClose={() => setShowDialog(false)}
 *   onConfirm={handleCreate}
 *   type="create"
 *   title="Create Product"
 *   message="Are you sure you want to create this product?"
 * />
 */
const ConfirmationDialog = ({
  isOpen,
  onClose,
  onConfirm,
  type = 'delete',
  title,
  message,
  itemName,
  confirmText,
  cancelText = 'Cancel',
  loading = false,
}) => {
  if (!isOpen) return null;

  // Configuration for different dialog types
  const typeConfig = {
    delete: {
      icon: Trash2,
      iconBgColor: 'bg-red-100',
      iconColor: 'text-red-600',
      confirmButtonBg: 'bg-red-600 hover:bg-red-700 active:bg-red-800',
      confirmButtonText: confirmText || 'Delete Product',
      headerBgColor: 'bg-red-50',
      borderColor: 'border-red-200',
    },
    update: {
      icon: AlertCircle,
      iconBgColor: 'bg-blue-100',
      iconColor: 'text-blue-600',
      confirmButtonBg: 'bg-blue-600 hover:bg-blue-700 active:bg-blue-800',
      confirmButtonText: confirmText || 'Update Product',
      headerBgColor: 'bg-blue-50',
      borderColor: 'border-blue-200',
    },
    create: {
      icon: CheckCircle,
      iconBgColor: 'bg-green-100',
      iconColor: 'text-green-600',
      confirmButtonBg: 'bg-green-600 hover:bg-green-700 active:bg-green-800',
      confirmButtonText: confirmText || 'Create Product',
      headerBgColor: 'bg-green-50',
      borderColor: 'border-green-200',
    },
  };

  const config = typeConfig[type] || typeConfig.delete;
  const IconComponent = config.icon;

  const handleBackdropClick = e => {
    if (e.target === e.currentTarget) {
      onClose();
    }
  };

  const handleCancel = () => {
    onClose();
  };

  const handleConfirm = () => {
    onConfirm();
  };

  return (
    <div
      className='fixed inset-0 z-[9999] flex items-center justify-center bg-black/50 backdrop-blur-sm'
      onClick={handleBackdropClick}
      role='dialog'
      aria-modal='true'
      aria-labelledby='dialog-title'
    >
      <div
        className='relative w-full max-w-md mx-4 bg-white rounded-xl shadow-2xl transform transition-all'
        onClick={e => e.stopPropagation()}
      >
        {/* Header with Icon */}
        <div
          className={`${config.headerBgColor} ${config.borderColor} border-b rounded-t-xl p-6`}
        >
          <div className='flex items-start gap-4'>
            {/* Icon */}
            <div
              className={`${config.iconBgColor} rounded-full p-3 flex-shrink-0`}
            >
              <IconComponent className={`w-6 h-6 ${config.iconColor}`} />
            </div>

            {/* Title and Item Name */}
            <div className='flex-1 min-w-0'>
              <h2
                id='dialog-title'
                className='text-xl font-semibold text-gray-900 mb-1'
              >
                {title}
              </h2>
              {itemName && (
                <p className='text-base font-medium text-gray-700'>
                  {itemName}
                </p>
              )}
            </div>
          </div>
        </div>

        {/* Message Content */}
        <div className='p-6'>
          <p className='text-sm text-gray-600 leading-relaxed'>
            {itemName ? (
              <>
                Are you sure you want to {type} "{itemName}"? {message}
              </>
            ) : (
              message
            )}
          </p>
        </div>

        {/* Action Buttons */}
        <div className='flex items-center justify-end gap-3 px-6 py-4 bg-gray-50 rounded-b-xl border-t border-gray-200'>
          {/* Cancel Button */}
          <button
            onClick={handleCancel}
            disabled={loading}
            className='px-4 cursor-pointer py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded-lg hover:bg-gray-50 active:bg-gray-100 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-gray-500 transition-colors disabled:opacity-50 disabled:cursor-not-allowed'
            type='button'
          >
            {cancelText}
          </button>

          {/* Confirm Button */}
          <button
            onClick={handleConfirm}
            disabled={loading}
            className={`px-4 py-2 text-sm cursor-pointer font-medium text-white ${config.confirmButtonBg} rounded-lg focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-${type === 'delete' ? 'red' : type === 'update' ? 'blue' : 'green'}-500 transition-colors disabled:opacity-50 disabled:cursor-not-allowed flex items-center gap-2`}
            type='button'
          >
            {loading && (
              <svg
                className='animate-spin h-4 w-4 text-white'
                xmlns='http://www.w3.org/2000/svg'
                fill='none'
                viewBox='0 0 24 24'
              >
                <circle
                  className='opacity-25'
                  cx='12'
                  cy='12'
                  r='10'
                  stroke='currentColor'
                  strokeWidth='4'
                />
                <path
                  className='opacity-75'
                  fill='currentColor'
                  d='M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z'
                />
              </svg>
            )}
            {config.confirmButtonText}
          </button>
        </div>
      </div>
    </div>
  );
};

export default ConfirmationDialog;
