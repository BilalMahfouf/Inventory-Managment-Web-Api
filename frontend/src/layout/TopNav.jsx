import NotificationBell from '@/components/notification/NotificationBell';
import { Search, Bell, Menu } from 'lucide-react';

export default function TopNav({ onToggleSidebar }) {
  return (
    <nav className='fixed top-0 right-0 z-40 bg-white border-b border-gray-200 h-16 left-0 lg:left-64'>
      <div className='flex items-center justify-between h-full px-4 lg:px-6'>
        {/* Left side - Mobile menu button (tablet & mobile) and Search (desktop/tablet) */}
        <div className='flex items-center space-x-4 flex-1'>
          {/* Mobile menu button for tablet and mobile */}
          <button
            onClick={onToggleSidebar}
            className='lg:hidden text-gray-600 hover:text-gray-900 p-2'
          >
            <Menu className='h-6 w-6' />
          </button>

          {/* Search Bar - hidden on mobile, visible on tablet and desktop */}
          <div className='hidden md:block flex-1 max-w-lg'>
            <div className='relative'>
              <div className='absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none'>
                <Search className='h-5 w-5 text-gray-400' />
              </div>
              <input
                type='text'
                placeholder='Search products, orders, customers...'
                className='block w-full pl-10 pr-3 py-2.5 border border-gray-200 rounded-lg leading-5 bg-gray-50 placeholder-gray-500 focus:outline-none focus:placeholder-gray-400 focus:ring-1 focus:ring-blue-500 focus:border-blue-500 focus:bg-white text-sm'
              />
            </div>
          </div>
        </div>

        {/* Right Side - Notifications and User (always visible) */}
        <div className='flex items-center space-x-4 md:space-x-6'>
          {/* Notifications */}
          <NotificationBell />

          {/* User Profile */}
          <div className='flex items-center space-x-2'>
            <div className='flex items-center justify-center h-8 w-8 rounded-full bg-gray-700 text-white font-semibold text-sm'>
              JD
            </div>
          </div>
        </div>
      </div>
    </nav>
  );
}
