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
  Truck,
  BadgeCheck,
} from 'lucide-react';
import PageHeader from '../components/ui/PageHeader';
import Button from '@components/Buttons/Button';
import { fetchWithAuth } from '../services/auth/authService';
import QuickActions from '@components/ui/quickAction/QuickActions';
import Alerts from '../components/ui/alerts/Alerts';
import TopSellingProducts from '../components/ui/TopSellingProducts';
import TodaysPerformanceContainer from '../components/ui/TodaysPerformanceContainer';
import AddProductButton from '@/components/products/AddProductButton';

export default function DashboardPage() {
  const [activeProducts, setActiveProducts] = useState(0);
  const [activeCustomers, setActiveCustomers] = useState(0);
  const [lowStockProducts, setLowStockProducts] = useState(0);
  const [totalSalesOrders, setTotalSalesOrders] = useState(0);
  const [totalRevenues, setTotalRevenues] = useState(0);
  const [pendingSalesOrders, setPendingSalesOrders] = useState(0);
  const [activeSuppliers, setActiveSuppliers] = useState(0);
  const [completedSalesOrders, setCompletedSalesOrders] = useState(0);

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
          totalProducts,
          lowStockProducts,
          activeCustomers,
          totalSalesOrders,
          totalRevenues,
          pendingSalesOrders,
          activeSuppliers,
          completedSalesOrders,
        } = await result.response.json();
        setLoading(false);
        setActiveProducts(totalProducts);
        setActiveCustomers(activeCustomers);
        setLowStockProducts(lowStockProducts);
        setTotalSalesOrders(totalSalesOrders);
        setTotalRevenues(totalRevenues);
        setPendingSalesOrders(pendingSalesOrders);
        setActiveSuppliers(activeSuppliers);
        setCompletedSalesOrders(completedSalesOrders);
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

        <div className='flex-1 md:justify-end flex gap-2'>
          <Button variant='secondary' className='text-sm' LeftIcon={FileText}>
            Generate Report
          </Button>
          <AddProductButton />
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
          number={loading ? '?' : `$${totalRevenues}`}
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
          number={loading ? '?' : pendingSalesOrders}
          description='total revenue of the bussiness'
          status={true}
          statusLabel='+8%'
          iconComponent={Clock3}
          iconClassName='text-orange-500'
          numberClassName='text-orange-500'
        />
        <InfoCard
          className='flex-1'
          title='Fulfilled Orders'
          number={loading ? '?' : completedSalesOrders}
          description='total revenue of the bussiness'
          status={true}
          statusLabel='+8%'
          iconComponent={BadgeCheck}
          iconClassName='text-green-500'
        />
        <InfoCard
          className='flex-1'
          title='Active Suppliers'
          number={loading ? '?' : activeSuppliers}
          description='total revenue of the bussiness'
          status={true}
          statusLabel='+8%'
          iconComponent={Truck}
        />
      </div>
      <TodaysPerformanceContainer className='mt-6' />

      <div className='mt-6 flex flex-col   lg:grid lg:grid-cols-3 xl:grid-cols-3 gap-6'>
        <div className='p-4 bg-white rounded-lg border-white shadow-sm flex-1'>
          <div className='mb-6 flex items-center gap-2'>
            <Flame className='w-6 h-6 text-black' />
            <h3 className='text-gray-900 font-semibold text-2xl'>
              Top Selling Products
            </h3>
          </div>
          <TopSellingProducts />
        </div>

        <div className='p-4 bg-white rounded-lg border-white shadow-sm flex-1'>
          <div className='mb-6 flex items-center gap-2'>
            <Zap className='w-6 h-6 text-black' />
            <h3 className='text-gray-900 font-semibold text-2xl'>
              Quick Actions
            </h3>
          </div>
          <QuickActions className='' />
        </div>

        <div className='p-4 bg-white rounded-lg border-white shadow-sm flex-1'>
          <div className='mb-6 flex items-center gap-2'>
            <TriangleAlert className='w-6 h-6 text-black' />
            <h3 className='text-gray-900 font-semibold text-2xl'>
              Inventory Alerts
            </h3>
          </div>
          <Alerts />
        </div>
      </div>
    </div>
  );
}
