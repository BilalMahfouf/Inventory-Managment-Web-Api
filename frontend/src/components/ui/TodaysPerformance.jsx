import React from 'react';
import { TrendingUp, ShoppingCart, Users, Package, Zap } from 'lucide-react';
import PerformanceCard from './PerformanceCard';

const TodaysPerformance = ({
  todaysSales,
  salesChange,
  newOrders,
  ordersChange,
  newCustomers,
  customersChange,
  productsSold,
  productsSoldChange,
  loading = false,
}) => {
  return (
    <div className=' bg-white rounded-lg border-white shadow-sm p-4 mt-6'>
      <div className='mb-6 flex items-center gap-2 '>
        <Zap className='w-6 h-6 text-black' />
        <h3 className='text-gray-900 font-semibold text-2xl'>
          Today's Performance
        </h3>
      </div>

      <div className='flex flex-col md:flex-row gap-4'>
        <PerformanceCard
          icon={TrendingUp}
          iconColor='text-green-500'
          value={todaysSales}
          label="Today's Sales"
          change={salesChange}
          loading={loading}
          className='flex-1'
        />

        <PerformanceCard
          icon={ShoppingCart}
          iconColor='text-blue-500'
          value={newOrders}
          label='New Orders'
          change={ordersChange}
          loading={loading}
          className='flex-1'
        />

        <PerformanceCard
          icon={Users}
          iconColor='text-purple-500'
          value={newCustomers}
          label='New Customers'
          change={customersChange}
          loading={loading}
          className='flex-1'
        />

        <PerformanceCard
          icon={Package}
          iconColor='text-orange-500'
          value={productsSold}
          label='Products Sold'
          change={productsSoldChange}
          loading={loading}
          className='flex-1'
        />
      </div>
    </div>
  );
};

export default TodaysPerformance;
