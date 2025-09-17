import { StrictMode, useEffect, useState } from 'react';
import InfoCard from '@/components/ui/InfoCard';
import {
  Clock3,
  DollarSign,
  FileText,
  Info,
  Package,
  Plus,
  ShoppingCart,
  TrendingDown,
  TrendingUp,
  TriangleAlert,
  Users2,
  Zap,
  Flame,
} from 'lucide-react';
import PageHeader from '../components/ui/PageHeader';
import Button from '@components/Buttons/Button';
import { fetchWithAuth } from '../services/auth/authService';
import QuickActions from '@components/ui/quickAction/QuickActions';
import Alerts from '../components/ui/alerts/Alerts';

export default function DashboardPage() {
  const [activeProducts, setActiveProducts] = useState(0);
  const [activeCustomers, setActiveCustomers] = useState(0);
  const [lowStockProducts, setLowStockProducts] = useState(0);
  const [totalSalesOrders, setTotalSalesOrders] = useState(0);
  //const [error, setError] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    setLoading(true);
    console.log('use effect runs ....');
    const fetchData = async () => {
      try {
        const result = await fetchWithAuth('api/dashboard/summary');
        if (!result.success) {
          console.error('Failed to fetch dashboard summary:', result.error);
          //  setError(result.error);
          setLoading(false);
          throw new Error(result.error);
        }
        console.log(result);

        const {
          activeProducts,
          lowStockProducts,
          activeCustomers,
          totalSalesOrders,
        } = await result.response.json();
        setLoading(false);
        setActiveProducts(activeProducts);
        setActiveCustomers(activeCustomers);
        setLowStockProducts(lowStockProducts);
        setTotalSalesOrders(totalSalesOrders);
      } catch (error) {
        console.error(
          'An error occurred while fetching dashboard data:',
          error
        );
        // setError(error.message);
        setLoading(false);
      }
    };
    fetchData();
  }, []);

  return (
    <div className=''>
      <div className='flex flex-col md:flex-row md:justify-between md:items-center mb-5'>
        <PageHeader
          title='Dashboard'
          description="Welcome back! Here's what's happening with your inventory."
          containerClass='sm: mb-4 flex-1'
        />

        <div className='flex-1 md:justify-end flex'>
          <Button variant='secondary' className='  text-sm' LeftIcon={FileText}>
            Generate Report
          </Button>
          <Button variant='primary' className='ml-4 text-sm w' LeftIcon={Plus}>
            Add Product
          </Button>
        </div>
      </div>

      <div className='flex flex-col w-full md:grid md:grid-cols-2 lg:grid lg:grid-cols-4 gap-6 '>
        <InfoCard
          className='flex-1'
          title='Total Products'
          number={loading ? '?' : activeProducts}
          description='active products'
          status={false}
          statusLabel='-12%'
          iconComponent={Package}
        />
        <InfoCard
          className='flex-1'
          title='Total Customers'
          number={loading ? '?' : activeCustomers}
          description='active customers'
          status={true}
          statusLabel='+12%'
          iconComponent={Users2}
        />
        <InfoCard
          className='flex-1'
          title='Low Stock Items'
          number={loading ? '?' : lowStockProducts}
          description='need restock'
          status={false}
          statusLabel='-4%'
          iconComponent={TriangleAlert}
        />
        <InfoCard
          className='flex-1'
          title='Total Sales Orders'
          number={loading ? '?' : totalSalesOrders}
          description='total sales of the bussiness'
          status={true}
          statusLabel='+8%'
          iconComponent={ShoppingCart}
        />
        <InfoCard
          className='flex-1'
          title='Total Revenues'
          number={loading ? '?' : totalSalesOrders}
          description='total revenue of the bussiness'
          status={true}
          statusLabel='+8%'
          iconComponent={DollarSign}
          iconClassName='text-green-500'
          numberClassName='text-green-700'
        />
        <InfoCard
          className='flex-1'
          title='Pending Orders'
          number={loading ? '?' : totalSalesOrders}
          description='total revenue of the bussiness'
          status={true}
          statusLabel='+8%'
          iconComponent={DollarSign}
          iconClassName='text-green-500'
          numberClassName='text-green-700'
        />
        <InfoCard
          className='flex-1'
          title='Fulfilled Orders'
          number={loading ? '?' : totalSalesOrders}
          description='total revenue of the bussiness'
          status={true}
          statusLabel='+8%'
          iconComponent={DollarSign}
          iconClassName='text-green-500'
          numberClassName='text-green-700'
        />
        <InfoCard
          className='flex-1'
          title='Active Suppliers'
          number={loading ? '?' : totalSalesOrders}
          description='total revenue of the bussiness'
          status={true}
          statusLabel='+8%'
          iconComponent={DollarSign}
          iconClassName='text-green-500'
          numberClassName='text-green-700'
        />
      </div>

      <div className='mt-6  flex flex-col lg:flex lg:flex-row gap-6'>
        <div className='p-4 bg-white rounded-lg border-white  shadow-sm flex-1 h-150'>
          <div className='mb-6 flex items-center gap-2'>
            <Flame className='w-6 h-6 text-black' />
            <h3 className='text-gray-900 font-semibold text-2xl'>
              {' '}
              Top Selling Products
            </h3>
          </div>
          this feature is not implement yet
        </div>

        <div className='p-4 bg-white rounded-lg border-white  shadow-sm flex-1'>
          <div className='mb-6 flex items-center gap-2'>
            <Zap className='w-6 h-6 text-black' />
            <h3 className='text-gray-900 font-semibold text-2xl'>
              {' '}
              Quick Actions
            </h3>
          </div>
          <QuickActions className='' />
        </div>

        <div className='p-4 bg-white rounded-lg border-white  shadow-sm flex-1'>
          <div className='mb-6 flex items-center gap-2'>
            <TriangleAlert className='w-6 h-6 text-black' />
            <h3 className='text-gray-900 font-semibold text-2xl'>
              {' '}
              Inventory Alerts
            </h3>
          </div>
          <Alerts />
        </div>
      </div>

      <div className='flex flex-col md:flex-row lg:flex-row gap-6 mt-6'>
        <InfoCard
          className='flex-1'
          title="Today's Sales"
          iconComponent={TrendingUp}
          titleClassName='text-gray-900'
          number='$14,579'
          description='sales today'
          iconClassName='text-green'
          numberClassName='text-green'
        />
        <InfoCard
          className='flex-1'
          title='Orders Today'
          iconComponent={ShoppingCart}
          titleClassName='text-gray-900'
          number='$14,579'
          description='orders today'
          iconClassName='text-green'
          numberClassName='text-green'
        />
        <InfoCard
          className='flex-1'
          title='Avg Order Value'
          iconComponent={TrendingDown}
          titleClassName='text-gray-900'
          number='$14,579'
          description='sales today'
          iconClassName='text-green'
          numberClassName='text-green'
        />
      </div>
    </div>
  );
}
