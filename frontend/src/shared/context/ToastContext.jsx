import { createContext, useContext, useState, useCallback } from 'react';

/**
 * ToastContext
 *
 * Provides a centralized toast notification system accessible throughout the app.
 * This context allows any component to show toast notifications without prop drilling.
 */
const ToastContext = createContext(null);

/**
 * ToastProvider Component
 *
 * Wraps the application to provide toast functionality to all child components.
 * Manages the global toast state and provides methods to show/hide notifications.
 *
 * @param {Object} props - Component props
 * @param {ReactNode} props.children - Child components
 */
export const ToastProvider = ({ children }) => {
  const [toasts, setToasts] = useState([]);

  /**
   * Generate a unique ID for each toast
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
   */
  const showSuccess = useCallback(
    (title, message = '', options = {}) => {
      //   console.log('showSuccess called:', title, message);
      return showToast('success', title, message, options);
    },
    [showToast]
  );

  /**
   * Show an error toast (red)
   */
  const showError = useCallback(
    (title, message = '', options = {}) => {
      console.log('showError called:', title, message);
      return showToast('error', title, message, options);
    },
    [showToast]
  );

  /**
   * Show a warning toast (orange)
   */
  const showWarning = useCallback(
    (title, message = '', options = {}) => {
      console.log('showWarning called:', title, message);
      return showToast('warning', title, message, options);
    },
    [showToast]
  );

  /**
   * Show an info toast (blue)
   */
  const showInfo = useCallback(
    (title, message = '', options = {}) => {
      return showToast('info', title, message, options);
    },
    [showToast]
  );

  /**
   * Remove a specific toast by ID
   */
  const removeToast = useCallback(id => {
    setToasts(prev => prev.filter(toast => toast.id !== id));
  }, []);

  /**
   * Remove all active toasts
   */
  const clearAllToasts = useCallback(() => {
    setToasts([]);
  }, []);

  const value = {
    toasts,
    showToast,
    showSuccess,
    showError,
    showWarning,
    showInfo,
    removeToast,
    clearAllToasts,
  };

  return (
    <ToastContext.Provider value={value}>{children}</ToastContext.Provider>
  );
};

/**
 * useToast Hook
 *
 * Custom hook to access toast functionality from any component.
 * Must be used within a ToastProvider.
 *
 * @returns {Object} Toast management methods
 *
 * @example
 * ```jsx
 * const { showSuccess, showError } = useToast();
 *
 * showSuccess('Saved!', 'Your changes have been saved.');
 * showError('Failed', 'Could not save changes.');
 * ```
 */
export const useToast = () => {
  const context = useContext(ToastContext);

  if (!context) {
    throw new Error('useToast must be used within a ToastProvider');
  }

  return context;
};

export default ToastContext;
