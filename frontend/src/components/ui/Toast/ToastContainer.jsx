import { createPortal } from 'react-dom';
import Toast from './Toast';

/**
 * ToastContainer Component
 *
 * A container component that manages and displays multiple toast notifications.
 * Handles positioning and stacking of toasts. Uses portal to render outside normal DOM hierarchy.
 *
 * @param {Object} props - Component props
 * @param {Array} props.toasts - Array of toast objects to display
 * @param {function} props.removeToast - Function to remove a toast by id
 * @param {string} props.position - Position of toast container:
 *   'top-right' (default), 'top-left', 'bottom-right', 'bottom-left', 'top-center', 'bottom-center'
 *
 * @example
 * ```jsx
 * const [toasts, setToasts] = useState([]);
 *
 * <ToastContainer
 *   toasts={toasts}
 *   removeToast={(id) => setToasts(toasts.filter(t => t.id !== id))}
 *   position="top-right"
 * />
 * ```
 */
const ToastContainer = ({
  toasts = [],
  removeToast,
  position = 'top-right',
}) => {
  // Position classes for different placements
  const positionClasses = {
    'top-right': 'top-4 right-4',
    'top-left': 'top-4 left-4',
    'bottom-right': 'bottom-4 right-4',
    'bottom-left': 'bottom-4 left-4',
    'top-center': 'top-4 left-1/2 -translate-x-1/2',
    'bottom-center': 'bottom-4 left-1/2 -translate-x-1/2',
  };

  const containerContent = (
    <div
      className={`fixed ${positionClasses[position]} z-[9999] flex flex-col gap-2 pointer-events-none`}
      aria-live='polite'
      aria-atomic='true'
    >
      {toasts.map(toast => (
        <div key={toast.id} className='pointer-events-auto'>
          <Toast
            type={toast.type}
            title={toast.title}
            message={toast.message}
            duration={toast.duration}
            showClose={toast.showClose}
            onClose={() => removeToast(toast.id)}
          />
        </div>
      ))}
    </div>
  );

  // Use portal to render outside the normal component tree
  return createPortal(containerContent, document.body);
};

export default ToastContainer;
