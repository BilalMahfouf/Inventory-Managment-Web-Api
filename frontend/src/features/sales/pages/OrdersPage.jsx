import { useNavigate } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { useTranslation } from 'react-i18next';
import {
  Plus,
  ShoppingCart,
  Clock,
  CheckCircle,
  DollarSign,
} from 'lucide-react';
import PageHeader from '@components/ui/PageHeader';
import InfoCard from '@components/ui/InfoCard';
import Button from '@components/Buttons/Button';
import OrdersDataTable from '@features/sales/components/OrdersDataTable';
import { getOrdersDahsobardSummary } from '@features/sales/services/salesOrderApi';
import { queryKeys } from '@shared/lib/queryKeys';
import i18nKeyContainer from '@shared/lib/i18n/keyContainer';

/**
 * OrdersPage Component
 *
 * Main orders list page displaying summary cards and filterable data table.
 *
 * @route /sales-orders
 */
export default function OrdersPage() {
  const { t, i18n } = useTranslation();
  const navigate = useNavigate();
  const activeLocale = i18n.resolvedLanguage || i18n.language || 'en';

  const { data: summaryResponse, isLoading: summaryLoading } = useQuery({
    queryKey: queryKeys.sales.summary(),
    queryFn: getOrdersDahsobardSummary,
  });

  const summary = summaryResponse?.isSuccess ? summaryResponse.data : null;
  const totalOrders = summary?.totalOrders ?? 0;
  const pendingOrders = summary?.pendingOrders ?? 0;
  const completedOrders =
    summary?.fulfilledOrders ?? summary?.completedOrders ?? 0;
  const totalRevenue = summary?.revenueThisMonth ?? summary?.totalRevenue ?? 0;

  return (
    <>
      {/* Header */}
      <div className='flex flex-col md:flex-row md:items-center md:justify-between mb-6 gap-4'>
        <PageHeader
          title={t(i18nKeyContainer.sales.orders.page.title)}
          description={t(i18nKeyContainer.sales.orders.page.description)}
        />

        <Button LeftIcon={Plus} onClick={() => navigate('/sales-orders/new')}>
          {t(i18nKeyContainer.sales.orders.page.newOrder)}
        </Button>
      </div>

      {/* Summary Cards */}
      <div className='flex flex-col md:flex-row gap-6 mb-6'>
        <InfoCard
          title={t(i18nKeyContainer.sales.orders.cards.totalOrders.title)}
          iconComponent={ShoppingCart}
          number={
            summaryLoading ? '...' : totalOrders.toLocaleString(activeLocale)
          }
          description={t(
            i18nKeyContainer.sales.orders.cards.totalOrders.description
          )}
          className='flex-1'
        />
        <InfoCard
          title={t(i18nKeyContainer.sales.orders.cards.pendingOrders.title)}
          iconComponent={Clock}
          number={
            summaryLoading ? '...' : pendingOrders.toLocaleString(activeLocale)
          }
          description={t(
            i18nKeyContainer.sales.orders.cards.pendingOrders.description
          )}
          className='flex-1'
        />
        <InfoCard
          title={t(i18nKeyContainer.sales.orders.cards.completedOrders.title)}
          iconComponent={CheckCircle}
          number={
            summaryLoading
              ? '...'
              : completedOrders.toLocaleString(activeLocale)
          }
          description={t(
            i18nKeyContainer.sales.orders.cards.completedOrders.description
          )}
          className='flex-1'
        />
        <InfoCard
          title={t(i18nKeyContainer.sales.orders.cards.revenueThisMonth.title)}
          iconComponent={DollarSign}
          number={
            summaryLoading
              ? '...'
              : `$${totalRevenue.toLocaleString(activeLocale, {
                  minimumFractionDigits: 2,
                  maximumFractionDigits: 2,
                })}`
          }
          description={t(
            i18nKeyContainer.sales.orders.cards.revenueThisMonth.description
          )}
          className='flex-1'
        />
      </div>

      {/* Orders Data Table */}
      <div className='bg-white rounded-lg shadow-sm border border-gray-200 p-6'>
        <OrdersDataTable />
      </div>
    </>
  );
}
