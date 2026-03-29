import { useQuery } from '@tanstack/react-query';
import InfoCard from '@components/ui/InfoCard';
import {
  Clock3,
  DollarSign,
  FileText,
  Package,
  ShoppingCart,
  TriangleAlert,
  Users2,
  Zap,
  Flame,
  Truck,
  BadgeCheck,
} from 'lucide-react';
import PageHeader from '@components/ui/PageHeader';
import Button from '@components/Buttons/Button';
import dashboardApi from '@features/dashboard/services/dashboardApi';
import { queryKeys } from '@shared/lib/queryKeys';
import QuickActions from '@features/dashboard/components/quickAction/QuickActions';
import Alerts from '@features/dashboard/components/alerts/Alerts';
import TopSellingProducts from '@features/dashboard/components/TopSellingProducts';
import TodaysPerformanceContainer from '@features/dashboard/components/TodaysPerformanceContainer';
import AddProductButton from '@features/products/components/products/AddProductButton';
import { useTranslation } from 'react-i18next';
import i18nKeyContainer from '@shared/lib/i18n/keyContainer';

export default function DashboardPage() {
  const { t } = useTranslation();
  const { data: summary, isLoading: loading } = useQuery({
    queryKey: queryKeys.dashboard.summary(),
    queryFn: dashboardApi.getSummary,
  });

  const activeProducts = summary?.totalProducts ?? 0;
  const activeCustomers = summary?.activeCustomers ?? 0;
  const lowStockProducts = summary?.lowStockProducts ?? 0;
  const totalSalesOrders = summary?.totalSalesOrders ?? 0;
  const totalRevenues = summary?.totalRevenues ?? 0;
  const pendingSalesOrders = summary?.pendingSalesOrders ?? 0;
  const activeSuppliers = summary?.activeSuppliers ?? 0;
  const completedSalesOrders = summary?.completedSalesOrders ?? 0;

  return (
    <div className=''>
      <div className='flex flex-col md:flex-row md:justify-between md:items-center mb-5'>
        <PageHeader
          title={t(i18nKeyContainer.dashboard.page.title)}
          description={t(i18nKeyContainer.dashboard.page.description)}
          containerClass='sm: mb-4 flex-1'
        />

        <div className='flex-1 md:justify-end flex gap-2'>
          <Button variant='secondary' className='text-sm' LeftIcon={FileText}>
            {t(i18nKeyContainer.dashboard.page.generateReport)}
          </Button>
          <AddProductButton />
        </div>
      </div>

      <div className='flex flex-col w-full md:grid md:grid-cols-2 lg:grid lg:grid-cols-4 gap-6 '>
        <InfoCard
          className='flex-1'
          title={t(i18nKeyContainer.dashboard.cards.totalProducts.title)}
          number={loading ? '?' : activeProducts}
          description={t(i18nKeyContainer.dashboard.cards.totalProducts.description)}
          status={false}
          statusLabel='-12%'
          iconComponent={Package}
        />
        <InfoCard
          className='flex-1'
          title={t(i18nKeyContainer.dashboard.cards.totalCustomers.title)}
          number={loading ? '?' : activeCustomers}
          description={t(i18nKeyContainer.dashboard.cards.totalCustomers.description)}
          status={true}
          statusLabel='+12%'
          iconComponent={Users2}
        />
        <InfoCard
          className='flex-1'
          title={t(i18nKeyContainer.dashboard.cards.lowStockItems.title)}
          number={loading ? '?' : lowStockProducts}
          description={t(i18nKeyContainer.dashboard.cards.lowStockItems.description)}
          status={false}
          statusLabel='-4%'
          iconComponent={TriangleAlert}
        />
        <InfoCard
          className='flex-1'
          title={t(i18nKeyContainer.dashboard.cards.totalSalesOrders.title)}
          number={loading ? '?' : totalSalesOrders}
          description={t(i18nKeyContainer.dashboard.cards.totalSalesOrders.description)}
          status={true}
          statusLabel='+8%'
          iconComponent={ShoppingCart}
        />
        <InfoCard
          className='flex-1'
          title={t(i18nKeyContainer.dashboard.cards.totalRevenues.title)}
          number={loading ? '?' : `$${totalRevenues}`}
          description={t(i18nKeyContainer.dashboard.cards.totalRevenues.description)}
          status={true}
          statusLabel='+8%'
          iconComponent={DollarSign}
          iconClassName='text-green-500'
          numberClassName='text-green-700'
        />
        <InfoCard
          className='flex-1'
          title={t(i18nKeyContainer.dashboard.cards.pendingOrders.title)}
          number={loading ? '?' : pendingSalesOrders}
          description={t(i18nKeyContainer.dashboard.cards.pendingOrders.description)}
          status={true}
          statusLabel='+8%'
          iconComponent={Clock3}
          iconClassName='text-orange-500'
          numberClassName='text-orange-500'
        />
        <InfoCard
          className='flex-1'
          title={t(i18nKeyContainer.dashboard.cards.fulfilledOrders.title)}
          number={loading ? '?' : completedSalesOrders}
          description={t(i18nKeyContainer.dashboard.cards.fulfilledOrders.description)}
          status={true}
          statusLabel='+8%'
          iconComponent={BadgeCheck}
          iconClassName='text-green-500'
        />
        <InfoCard
          className='flex-1'
          title={t(i18nKeyContainer.dashboard.cards.activeSuppliers.title)}
          number={loading ? '?' : activeSuppliers}
          description={t(i18nKeyContainer.dashboard.cards.activeSuppliers.description)}
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
              {t(i18nKeyContainer.dashboard.sections.topSellingProducts)}
            </h3>
          </div>
          <TopSellingProducts />
        </div>

        <div className='p-4 bg-white rounded-lg border-white shadow-sm flex-1'>
          <div className='mb-6 flex items-center gap-2'>
            <Zap className='w-6 h-6 text-black' />
            <h3 className='text-gray-900 font-semibold text-2xl'>
              {t(i18nKeyContainer.dashboard.sections.quickActions)}
            </h3>
          </div>
          <QuickActions className='' />
        </div>

        <div className='p-4 bg-white rounded-lg border-white shadow-sm flex-1'>
          <div className='mb-6 flex items-center gap-2'>
            <TriangleAlert className='w-6 h-6 text-black' />
            <h3 className='text-gray-900 font-semibold text-2xl'>
              {t(i18nKeyContainer.dashboard.sections.inventoryAlerts)}
            </h3>
          </div>
          <Alerts />
        </div>
      </div>
    </div>
  );
}
