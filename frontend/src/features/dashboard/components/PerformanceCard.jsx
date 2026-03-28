import React from 'react';

const PerformanceCard = ({
  icon,
  iconColor = 'text-gray-500',
  value,
  label,
  change,
  loading = false,
  className = '',
}) => {
  const IconComponent = icon;
  const isPositive = change?.startsWith('+');
  const changeColor = isPositive ? 'text-green-500' : 'text-red-500';

  return (
    <div
      className={`bg-gray-50 border border-gray-200 rounded-lg p-4 ${className}`}
    >
      <div className='flex items-start gap-3'>
        {/* Icon */}
        <div className={`${iconColor} mt-1 text-center`}>
          <IconComponent className='w-5 h-5' />
        </div>

        {/* Content */}
        <div className='flex-1'>
          <div className='text-xl font-bold text-gray-900 mb-1'>
            {loading ? '...' : value}
          </div>
          <div className='text-sm text-gray-600 mb-1'>{label}</div>
          <div className={`text-sm font-medium ${changeColor}`}>
            {loading ? '...' : change}
          </div>
        </div>
      </div>
    </div>
  );
};

export default PerformanceCard;
