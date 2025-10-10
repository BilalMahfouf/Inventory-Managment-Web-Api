import { useState, useCallback } from 'react';

/**
 * useToast Hook
 *
 * A custom hook for managing toast notifications in your application.
 * Provides methods to show success, error, warning, and info toasts.
 * Generic and reusable - works with any operation, not just products.
 *
 * @returns {Object} Toast management object
 * @returns {Array} toasts - Array of active toast notifications
 * @returns {Function} showToast - Generic function to show any type of toast
 * @returns {Function} showSuccess - Show success toast (green)
 * @returns {Function} showError - Show error toast (red)
 * @returns {Function} showWarning - Show warning toast (orange)
 * @returns {Function} showInfo - Show info toast (blue)
 * @returns {Function} removeToast - Remove a specific toast by id
 * @returns {Function} clearAllToasts - Remove all active toasts
 *
 * @example
 * ```jsx
 * const { toasts, showSuccess, showError, removeToast } = useToast();
 *
 * // Show success toast
 * showSuccess('Product Created Successfully', 'The product has been added.');
 *
 * // Show error toast
 * showError('Update Failed', 'Product could not be updated.');
 *
 * // Show toast with custom duration
 * showSuccess('Saved!', 'Changes saved.', { duration: 3000 });
 * ```
 */
const useToast = () => {
  const [toasts, setToasts] = useState([]);

  /**
   * Generate a unique ID for each toast
   *
   * @returns {string} Unique toast ID
   */
  const generateId = () => {
    return `toast-${Date.now()}-${Math.random().toString(36).substr(2, 9)}`;
  };

  /**
   * Show a generic toast notification
   *
   * @param {string} type - Toast type: 'success', 'error', 'warning', 'info'
   * @param {string} title - Toast title
   * @param {string} message - Toast message (optional)
   * @param {Object} options - Additional options
   * @param {number|null} options.duration - Auto-dismiss duration in ms (default: 5000, null = no auto-dismiss)
   * @param {boolean} options.showClose - Show close button (default: true)
   * @returns {string} Toast ID
   */
  const showToast = useCallback((type, title, message = '', options = {}) => {
    const id = generateId();
    const newToast = {
      id,
      type,
      title,
      message,
      duration: options.duration ?? 5000,
      showClose: options.showClose ?? true,
    };

    setToasts(prev => [...prev, newToast]);

    return id;
  }, []);

  /**
   * Show a success toast (green)
   * Use for: successful operations, confirmations
   *
   * @param {string} title - Toast title
   * @param {string} message - Toast message (optional)
   * @param {Object} options - Additional options
   * @returns {string} Toast ID
   *
   * @example
   * showSuccess('Product Created Successfully', 'AirPods Pro has been added.');
   */
  const showSuccess = useCallback(
    (title, message = '', options = {}) => {
      return showToast('success', title, message, options);
    },
    [showToast]
  );

  /**
   * Show an error toast (red)
   * Use for: failed operations, validation errors, critical issues
   *
   * @param {string} title - Toast title
   * @param {string} message - Toast message (optional)
   * @param {Object} options - Additional options
   * @returns {string} Toast ID
   *
   * @example
   * showError('Update Failed', 'Product could not be updated.');
   */
  const showError = useCallback(
    (title, message = '', options = {}) => {
      return showToast('error', title, message, options);
    },
    [showToast]
  );

  /**
   * Show a warning toast (orange)
   * Use for: important information, warnings, non-critical issues
   *
   * @param {string} title - Toast title
   * @param {string} message - Toast message (optional)
   * @param {Object} options - Additional options
   * @returns {string} Toast ID
   *
   * @example
   * showWarning('Low Stock', 'Only 2 units remaining.');
   */
  const showWarning = useCallback(
    (title, message = '', options = {}) => {
      return showToast('warning', title, message, options);
    },
    [showToast]
  );

  /**
   * Show an info toast (blue)
   * Use for: general information, status updates, tips
   *
   * @param {string} title - Toast title
   * @param {string} message - Toast message (optional)
   * @param {Object} options - Additional options
   * @returns {string} Toast ID
   *
   * @example
   * showInfo('Processing', 'Your request is being processed.');
   */
  const showInfo = useCallback(
    (title, message = '', options = {}) => {
      return showToast('info', title, message, options);
    },
    [showToast]
  );

  /**
   * Remove a specific toast by ID
   *
   * @param {string} id - Toast ID to remove
   *
   * @example
   * const toastId = showSuccess('Processing...');
   * // Later...
   * removeToast(toastId);
   */
  const removeToast = useCallback(id => {
    setToasts(prev => prev.filter(toast => toast.id !== id));
  }, []);

  /**
   * Remove all active toasts
   * Useful for clearing notifications when navigating or on specific events
   *
   * @example
   * clearAllToasts();
   */
  const clearAllToasts = useCallback(() => {
    setToasts([]);
  }, []);

  return {
    toasts,
    showToast,
    showSuccess,
    showError,
    showWarning,
    showInfo,
    removeToast,
    clearAllToasts,
  };
};

export default useToast;
