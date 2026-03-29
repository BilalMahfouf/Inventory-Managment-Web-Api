import CustomerDataTable from '@features/customers/components/customers/CustomerDataTable';
import InfoCard from '@/components/ui/InfoCard';
import PageHeader from '@/components/ui/PageHeader';
import Button from '@components/Buttons/Button';
import { getCustomerSummary } from '@features/customers/services/customerApi';
import { divStyles } from '@shared/utils/uiVariables';
import { ArrowRight, DollarSign, User2 } from 'lucide-react';
import { useQuery } from '@tanstack/react-query';
import { Tab, TabList, TabPanel, Tabs } from 'react-tabs';
import { AddCustomerButton } from '@features/customers/components/customers';
import { useTranslation } from 'react-i18next';
import i18nKeyContainer from '@shared/lib/i18n/keyContainer';
import { queryKeys } from '@shared/lib/queryKeys';
import { useNavigate } from 'react-router-dom';

export default function CustomersPage() {
  const { t, i18n } = useTranslation();
  const navigate = useNavigate();
  const { data: summaryResponse, isLoading: loading } = useQuery({
    queryKey: queryKeys.customers.summary(),
    queryFn: getCustomerSummary,
  });

  const activeLocale = i18n.resolvedLanguage || i18n.language || 'en';

  const summary = summaryResponse?.success ? summaryResponse.data : null;
  const totalCustomers = summary?.totalCustomers ?? 0;
  const totalRevenue = summary?.totalRevenue ?? 0;
  const newCustomersLastMonth = summary?.newCustomersLastMonth ?? 0;
  const activeCustomers = summary?.activeCustomers ?? 0;

  return (
    <>
      <div className='flex flex-col md:flex-row md:items-center md:justify-between mb-6 gap-4'>
        <PageHeader
          title={t(i18nKeyContainer.customers.page.title)}
          description={t(i18nKeyContainer.customers.page.description)}
        />
        <AddCustomerButton />
      </div>
      <div className='flex flex-col md:flex-row gap-6'>
        <InfoCard
          title={t(i18nKeyContainer.customers.cards.totalCustomers.title)}
          iconComponent={User2}
          number={
            loading
              ? t(i18nKeyContainer.customers.shared.loading)
              : totalCustomers.toLocaleString(activeLocale)
          }
          description={t(
            i18nKeyContainer.customers.cards.totalCustomers.description,
            {
              count: activeCustomers.toLocaleString(activeLocale),
            }
          )}
          className='flex-1'
        />
        <InfoCard
          title={t(i18nKeyContainer.customers.cards.totalRevenue.title)}
          iconComponent={DollarSign}
          number={
            loading
              ? t(i18nKeyContainer.customers.shared.loading)
              : `${t(i18nKeyContainer.customers.shared.currencySymbol)}${totalRevenue.toLocaleString(activeLocale, { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`
          }
          description={t(i18nKeyContainer.customers.cards.totalRevenue.description)}
          className='flex-1'
        />
        <InfoCard
          title={t(i18nKeyContainer.customers.cards.newCustomers.title)}
          iconComponent={User2}
          number={
            loading
              ? t(i18nKeyContainer.customers.shared.loading)
              : newCustomersLastMonth.toLocaleString(activeLocale)
          }
          description={t(i18nKeyContainer.customers.cards.newCustomers.description)}
          className='flex-1'
        />
        <InfoCard
          title={t(i18nKeyContainer.customers.cards.outOfStockItems.title)}
          iconComponent={User2}
          number={
            loading
              ? t(i18nKeyContainer.customers.shared.loading)
              : t(i18nKeyContainer.customers.cards.outOfStockItems.value)
          }
          description={t(i18nKeyContainer.customers.cards.outOfStockItems.description)}
          className='flex-1'
        />
      </div>
      <div>
        <Tabs>
          <TabList className='flex bg-blue-50 rounded-lg p-1 mb-4 mt-6 gap-1 max-w-fit border-0'>
            <Tab
              className='px-4 py-2 rounded-md cursor-pointer text-gray-600 transition-colors hover:text-gray-800'
              selectedClassName='!bg-white !text-gray-900 shadow-sm'
            >
              {t(i18nKeyContainer.customers.page.tabs.customers)}
            </Tab>
            <Tab
              className='px-4 py-2 rounded-md cursor-pointer text-gray-600 transition-colors hover:text-gray-800'
              selectedClassName='!bg-white !text-gray-900 shadow-sm'
            >
              {t(i18nKeyContainer.customers.page.tabs.customerCategories)}
            </Tab>
            <Tab
              className='px-4 py-2 rounded-md cursor-pointer text-gray-600 transition-colors hover:text-gray-800'
              selectedClassName='!bg-white !text-gray-900 shadow-sm'
            >
              {t(i18nKeyContainer.customers.page.tabs.customerContacts)}
            </Tab>
          </TabList>
          <div className={divStyles + 'mt-6'}>
            <TabPanel>
              <div className='mb-9 flex items-center justify-between'>
                <h3 className='text-2xl font-semibold leading-none tracking-tight'>
                  {t(i18nKeyContainer.customers.page.sections.customerCatalog)}
                </h3>
              </div>
              <CustomerDataTable />
            </TabPanel>
            <TabPanel>
              <div className='mb-9 flex items-center justify-between'>
                <h3 className='text-2xl font-semibold leading-none tracking-tight'>
                  {t(i18nKeyContainer.customers.page.sections.customerCategories)}
                </h3>
              </div>
              <div className='flex flex-col md:flex-row md:items-center md:justify-between gap-4'>
                <p>
                  {t(i18nKeyContainer.customers.page.placeholders.customerCategories)}
                </p>
                <Button
                  LeftIcon={ArrowRight}
                  onClick={() => {
                    navigate('/customers/categories');
                  }}
                >
                  {t(i18nKeyContainer.customers.page.actions.manageCustomerCategories)}
                </Button>
              </div>
            </TabPanel>
            <TabPanel>
              <div className='mb-9 flex items-center justify-between'>
                <h3 className='text-2xl font-semibold leading-none tracking-tight'>
                  {t(i18nKeyContainer.customers.page.sections.customerContacts)}
                </h3>
              </div>
              <div>
                {t(i18nKeyContainer.customers.page.placeholders.customerContacts)}
              </div>
            </TabPanel>
          </div>
        </Tabs>
      </div>
    </>
  );
}
