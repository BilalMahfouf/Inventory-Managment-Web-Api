import { useState, useRef, useEffect } from 'react';
import { Bell, X, AlertTriangle, CheckCircle, Info } from 'lucide-react';
import useSignalR from '@/signalr/useSignalR';

const test = {
  //   id: 1,
  //   severity: 'warning',
  //   title: 'Low Stock Alert',
  //   message: 'iPhone 15 Pro has only 5 units remaining',
  //   createdAt: '5 minutes ago',
  //   isRead: false,
};

const NotificationBell = () => {
  const [isOpen, setIsOpen] = useState(false);
  const [notifications, setNotifications] = useState([]);

  const dropdownRef = useRef(null);
  const unreadCount = notifications.filter(n => !n.isRead).length;

  // Close dropdown when clicking outside
  useEffect(() => {
    const handleClickOutside = event => {
      if (dropdownRef.current && !dropdownRef.current.contains(event.target)) {
        setIsOpen(false);
      }
    };

    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  const getIcon = severity => {
    switch (severity) {
      case 'warning':
        return <AlertTriangle className='w-5 h-5 text-orange-500' />;
      case 'success':
        return <CheckCircle className='w-5 h-5 text-green-500' />;
      case 'info':
        return <Info className='w-5 h-5 text-blue-500' />;
      default:
        return <Info className='w-5 h-5 text-gray-500' />;
    }
  };

  const getIconBg = severity => {
    switch (severity) {
      case 'warning':
        return 'bg-orange-50';
      case 'success':
        return 'bg-green-50';
      case 'info':
        return 'bg-blue-50';
      default:
        return 'bg-gray-50';
    }
  };

  const markAllAsRead = () => {
    setNotifications(notifications.map(n => ({ ...n, isRead: true })));
  };

  const removeNotification = id => {
    setNotifications(notifications.filter(n => n.id !== id));
  };

  const addNotification = notification => {
    const newNotification = {
      id: notification.id,
      severity: notification.severity,
      title: notification.title,
      message: notification.message,
      createdAt: notification.createdAt,
      isRead: false,
    };
    setNotifications(prevNotifications => [
      newNotification,
      ...prevNotifications,
    ]);
    notificationProps.remove(notification.id);
  };

  const notificationProps = useSignalR('low-stock-alert');
  if (notificationProps.messages && notificationProps.messages.length > 0) {
    notificationProps.messages.forEach(msg => addNotification(msg));
  }

  return (
    <div className='relative' ref={dropdownRef}>
      {/* Bell Icon Button */}
      <button
        onClick={() => setIsOpen(!isOpen)}
        className='relative p-2 cursor-pointer text-gray-600 hover:text-gray-800 hover:bg-gray-100 rounded-lg transition-colors'
        aria-label='Notifications'
      >
        <Bell className='w-6 h-6' />
        {unreadCount > 0 && (
          <span className='absolute top-0 right-0 flex items-center justify-center w-5 h-5 text-xs font-semibold text-white bg-red-500 rounded-full'>
            {unreadCount}
          </span>
        )}
      </button>

      {/* Notification Dropdown */}
      {isOpen && (
        <div className='absolute right-0 mt-2 w-96 bg-white rounded-lg shadow-lg border border-gray-200 z-50 overflow-hidden'>
          {/* Header */}
          <div className='flex items-center justify-between px-4 py-3 border-b border-gray-200'>
            <h3 className='text-lg font-semibold text-gray-900'>
              Notifications
            </h3>
            {notifications.length > 0 && (
              <button
                onClick={markAllAsRead}
                className='flex items-center gap-1 text-sm text-blue-600 hover:text-blue-700 font-medium'
              >
                <CheckCircle className='w-4 h-4' />
                Mark all read
              </button>
            )}
          </div>

          {/* Notifications List */}
          <div className='max-h-96 overflow-y-auto'>
            {notifications.length === 0 ? (
              <div className='px-4 py-8 text-center text-gray-500'>
                <Bell className='w-12 h-12 mx-auto mb-2 text-gray-300' />
                <p>No notifications</p>
              </div>
            ) : (
              notifications.map(notification => (
                <div
                  key={notification.id}
                  className={`relative px-4 py-3 border-b border-gray-100 hover:bg-gray-50 transition-colors ${
                    !notification.isRead ? 'bg-blue-50/30' : ''
                  }`}
                >
                  {/* Unread indicator */}
                  {!notification.isRead && (
                    <div className='absolute left-0 top-0 bottom-0 w-1 bg-blue-500'></div>
                  )}

                  <div className='flex gap-3'>
                    {/* Icon */}
                    <div
                      className={`flex-shrink-0 w-10 h-10 rounded-full ${getIconBg(notification.type)} flex items-center justify-center`}
                    >
                      {getIcon(notification.type)}
                    </div>

                    {/* Content */}
                    <div className='flex-1 min-w-0'>
                      <div className='flex items-start justify-between gap-2'>
                        <h4 className='text-sm font-semibold text-gray-900'>
                          {notification.title}
                        </h4>
                        <button
                          onClick={() => removeNotification(notification.id)}
                          className='flex-shrink-0 text-gray-400 hover:text-gray-600 transition-colors'
                          aria-label='Remove notification'
                        >
                          <X className='w-4 h-4' />
                        </button>
                      </div>
                      <p className='text-sm text-gray-600 mt-1'>
                        {notification.message}
                      </p>
                      <p className='text-xs text-gray-400 mt-1'>
                        {notification.time}
                      </p>
                    </div>
                  </div>
                </div>
              ))
            )}
          </div>
        </div>
      )}
    </div>
  );
};

export default NotificationBell;
