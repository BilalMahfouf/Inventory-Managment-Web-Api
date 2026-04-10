import { useNavigate } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { useTranslation } from 'react-i18next';
import { getOrdersDahsobardSummary } from '@features/sales/services/salesOrderApi';
import InfoCard from '@/components/ui/InfoCard';
import PageHeader from '@/components/ui/PageHeader';
import Button from '@/components/Buttons/Button';
import OrdersDataTable from '@features/sales/components/OrdersDataTable';
import {
  ShoppingCart,
  Clock,
  CheckCircle,
  DollarSign,
  Plus,
  ArrowRight,
} from 'lucide-react';
import { divStyles } from '@shared/utils/uiVariables';
import { queryKeys } from '@shared/lib/queryKeys';
import i18nKeyContainer from '@shared/lib/i18n/keyContainer';

export default function SalesPage() {
  const { t, i18n } = useTranslation();
  const navigate = useNavigate();
  const activeLocale = i18n.resolvedLanguage || i18n.language || 'en';

  const { data: summaryResponse, isLoading: loading } = useQuery({
    queryKey: queryKeys.sales.summary(),
    queryFn: getOrdersDahsobardSummary,
  });

  const summary = summaryResponse?.isSuccess ? summaryResponse.data : null;
  const totalOrders = summary?.totalOrders ?? 0;
  const pendingOrders = summary?.pendingOrders ?? 0;
  const completedOrders =
    summary?.fulfilledOrders ?? summary?.completedOrders ?? 0;
  const revenueThisMonth = summary?.revenueThisMonth ?? 0;

  return (
    <>
      <div className='flex flex-col md:flex-row md:items-center md:justify-between mb-6 gap-4'>
        <PageHeader
          title={t(i18nKeyContainer.sales.orders.page.title)}
          description={t(i18nKeyContainer.sales.orders.page.description)}
        />

        <div className='flex gap-2'>
          <Button
            variant='secondary'
            RightIcon={ArrowRight}
            onClick={() => navigate('/sales-orders')}
          >
            {t(i18nKeyContainer.sales.orders.page.viewAllOrders)}
          </Button>
          <Button LeftIcon={Plus} onClick={() => navigate('/sales-orders/new')}>
            {t(i18nKeyContainer.sales.orders.page.newOrder)}
          </Button>
        </div>
      </div>

      <div className='flex flex-col md:flex-row gap-6'>
        <InfoCard
          title={t(i18nKeyContainer.sales.orders.cards.totalOrders.title)}
          iconComponent={ShoppingCart}
          number={loading ? '...' : totalOrders.toLocaleString(activeLocale)}
          description={t(
            i18nKeyContainer.sales.orders.cards.totalOrders.description
          )}
          className='flex-1'
        />
        <InfoCard
          title={t(i18nKeyContainer.sales.orders.cards.pendingOrders.title)}
          iconComponent={Clock}
          number={loading ? '...' : pendingOrders.toLocaleString(activeLocale)}
          description={t(
            i18nKeyContainer.sales.orders.cards.pendingOrders.description
          )}
          className='flex-1'
        />
        <InfoCard
          title={t(i18nKeyContainer.sales.orders.cards.completedOrders.title)}
          iconComponent={CheckCircle}
          number={
            loading ? '...' : completedOrders.toLocaleString(activeLocale)
          }
          description={t(
            i18nKeyContainer.sales.orders.cards.completedOrders.description
          )}
          className='flex-1'
        />
        <InfoCard
          title={t(i18nKeyContainer.sales.orders.cards.totalRevenue.title)}
          iconComponent={DollarSign}
          number={
            loading
              ? '...'
              : `$${revenueThisMonth.toLocaleString(activeLocale, {
                  minimumFractionDigits: 2,
                  maximumFractionDigits: 2,
                })}`
          }
          description={t(
            i18nKeyContainer.sales.orders.cards.totalRevenue.description
          )}
          className='flex-1'
        />
      </div>

      {/* Recent Orders */}
      <div className={divStyles + 'mt-6'}>
        <div className='mb-6 flex items-center justify-between'>
          <h3 className='text-xl font-semibold leading-none tracking-tight'>
            {t(i18nKeyContainer.sales.orders.page.title)}
          </h3>
        </div>
        <OrdersDataTable />
      </div>
    </>
  );
}
