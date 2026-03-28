import React from 'react';

const TopProductItem = ({
  rank,
  productName,
  unitsSold,
  revenue,
  className = '',
}) => {
  return (
    <div
      className={`flex items-center justify-between p-3 hover:bg-gray-50
         rounded-lg transition-colors border border-gray-200    ${className}`}
    >
      {/* Left side - Rank and Product Info */}
      <div className='flex items-center gap-3'>
        {/* Rank Badge */}
        <div className='flex items-center justify-center w-8 h-8 bg-blue-100 text-blue-600 rounded-full text-sm font-semibold'>
          #{rank}
        </div>

        {/* Product Details */}
        <div className='flex flex-col'>
          <h4 className='text-gray-900 font-medium text-sm'>{productName}</h4>
          <p className='text-gray-500 text-xs'>{unitsSold} units sold</p>
        </div>
      </div>

      {/* Right side - Revenue */}
      <div className='text-right'>
        <p className='text-gray-900 font-semibold text-sm'>{revenue}</p>
        <p className='text-gray-400 text-xs'>Revenue</p>
      </div>
    </div>
  );
};

export default TopProductItem;
