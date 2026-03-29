import { useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import PageHeader from '@components/ui/PageHeader';
import InfoCard from '@components/ui/InfoCard';
import {
  DollarSign,
  Info,
  Package,
  Plus,
  TrendingUp,
  TriangleAlert,
} from 'lucide-react';
import Button from '@components/Buttons/Button';
import { getSummary } from '@features/products/services/productApi';
import { Tab, Tabs, TabList, TabPanel } from 'react-tabs';
import { useTranslation } from 'react-i18next';
import { divStyles } from '@shared/utils/uiVariables';
import i18nKeyContainer from '@shared/lib/i18n/keyContainer';
import ProductDataTable from '@features/products/components/ProductsTables/ProductDataTable';
import StockMovementHistoryTable from '@features/products/components/ProductsTables/StockMovementHistoryTable';
import { AddProduct } from '@features/products/components/products';
import UnitOfMeasureTable from '@features/products/components/unitOfMeasure/UnitOfMeasureTable';
import AddUnitOfMeasureButton from '@features/products/components/unitOfMeasure/AddUnitOfMeasureButton';
import ProductCategoryDataTable from '@features/products/components/productCategories/ProductCategoryDataTable';
import AddProductCategoryButton from '@features/products/components/productCategories/AddProductCategoryButton';
import { queryKeys } from '@shared/lib/queryKeys';
export default function ProductsPage() {
  const { t, i18n } = useTranslation();

  const { data: summary, isLoading: loading } = useQuery({
    queryKey: queryKeys.products.summary(),
    queryFn: getSummary,
  });
  const [isAddModalOpen, setIsAddModalOpen] = useState(false);

  const activeLocale = i18n.resolvedLanguage || i18n.language || 'en';

  const totalProductsCount = summary?.totalProducts ?? 0;
  const inventoryValue = summary?.inventoryValue ?? 0;
  const lowStockCount = summary?.lowStockProducts ?? 0;
  const profitPotential = summary?.profitPotential ?? 0;

  return (
    <div>
      <div className='flex flex-col md:flex-row md:items-center md:justify-between mb-6 gap-4'>
        <PageHeader
          title={t(i18nKeyContainer.products.page.title)}
          description={t(i18nKeyContainer.products.page.description)}
        />

        <Button LeftIcon={Plus} onClick={() => setIsAddModalOpen(true)}>
          {t(i18nKeyContainer.products.page.addProduct)}
        </Button>
      </div>
      <div className='flex flex-col md:flex-row gap-6'>
        <InfoCard
          title={t(i18nKeyContainer.products.cards.totalProducts.title)}
          iconComponent={Package}
          number={loading ? '...' : totalProductsCount.toLocaleString(activeLocale)}
          description={t(i18nKeyContainer.products.cards.totalProducts.description)}
          className='flex-1'
        />
        <InfoCard
          title={t(i18nKeyContainer.products.cards.inventoryValue.title)}
          iconComponent={DollarSign}
          number={
            loading
              ? '...'
              : `$${inventoryValue.toLocaleString(activeLocale, { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`
          }
          description={t(i18nKeyContainer.products.cards.inventoryValue.description)}
          className='flex-1'
          numberClassName='text-green-600'
          iconClassName='text-green-600'
        />
        <InfoCard
          title={t(i18nKeyContainer.products.cards.lowStockItems.title)}
          description={t(i18nKeyContainer.products.cards.lowStockItems.description)}
          number={loading ? '...' : lowStockCount}
          iconComponent={TriangleAlert}
          className='flex-1'
          numberClassName={lowStockCount > 0 ? 'text-red-600' : ''}
          iconClassName={lowStockCount > 0 ? 'text-red-600' : ''}
        />
        <InfoCard
          title={t(i18nKeyContainer.products.cards.profitPotential.title)}
          description={t(i18nKeyContainer.products.cards.profitPotential.description)}
          number={
            loading
              ? '...'
              : `$${profitPotential.toLocaleString(activeLocale, { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`
          }
          iconComponent={TrendingUp}
          className='flex-1'
        />
      </div>

      <Tabs>
        <TabList className='flex bg-blue-50 rounded-lg p-1 mb-4 mt-6 gap-1 max-w-fit border-0'>
          <Tab
            className='px-4 py-2 rounded-md cursor-pointer text-gray-600 transition-colors hover:text-gray-800'
            selectedClassName='!bg-white !text-gray-900 shadow-sm'
          >
            {t(i18nKeyContainer.products.page.tabs.products)}
          </Tab>
          <Tab
            className='px-4 py-2 rounded-md cursor-pointer text-gray-600 transition-colors hover:text-gray-800'
            selectedClassName='!bg-white !text-gray-900 shadow-sm'
          >
            {t(i18nKeyContainer.products.page.tabs.stockMovements)}
          </Tab>
          <Tab
            className='px-4 py-2 rounded-md cursor-pointer text-gray-600 transition-colors hover:text-gray-800'
            selectedClassName='!bg-white !text-gray-900 shadow-sm'
          >
            {t(i18nKeyContainer.products.page.tabs.unitOfMeasure)}
          </Tab>
          <Tab
            className='px-4 py-2 rounded-md cursor-pointer text-gray-600 transition-colors hover:text-gray-800'
            selectedClassName='!bg-white !text-gray-900 shadow-sm'
          >
            {t(i18nKeyContainer.products.page.tabs.productCategories)}
          </Tab>
          <Tab
            className='px-4 py-2 rounded-md cursor-pointer text-gray-600 transition-colors hover:text-gray-800'
            selectedClassName='!bg-white !text-gray-900 shadow-sm'
          >
            {t(i18nKeyContainer.products.page.tabs.productImages)}
          </Tab>
        </TabList>
        <div className={divStyles + 'mt-6'}>
          <TabPanel>
            <div className='mb-9 flex items-center justify-between'>
              <h3 className='text-2xl font-semibold leading-none tracking-tight'>
                {t(i18nKeyContainer.products.page.sections.productCatalog)}
              </h3>
            </div>
            <ProductDataTable />
          </TabPanel>
          <TabPanel>
            <div className='mb-9 flex items-center justify-between'>
              <h3 className='text-2xl font-semibold leading-none tracking-tight'>
                {t(i18nKeyContainer.products.page.sections.stockMovementHistory)}
              </h3>
            </div>
            <StockMovementHistoryTable />
          </TabPanel>
          <TabPanel>
            <div className='mb-9 flex items-center justify-between'>
              <h3 className='text-2xl font-semibold leading-none tracking-tight'>
                {t(i18nKeyContainer.products.page.sections.unitOfMeasure)}
              </h3>
              <AddUnitOfMeasureButton />
            </div>
            <UnitOfMeasureTable />
          </TabPanel>
          <TabPanel>
            <div className='mb-9 flex items-center justify-between'>
              <h3 className='text-2xl font-semibold leading-none tracking-tight'>
                {t(i18nKeyContainer.products.page.sections.productCategories)}
              </h3>
              <AddProductCategoryButton />
            </div>
            <ProductCategoryDataTable />
          </TabPanel>
          <TabPanel>
            <div className='mb-9 flex items-center justify-between'>
              <h3 className='text-2xl font-semibold leading-none tracking-tight'>
                {t(i18nKeyContainer.products.page.sections.productImages)}
              </h3>
            </div>
            <div>{t(i18nKeyContainer.products.page.placeholders.productImages)}</div>
          </TabPanel>
        </div>
      </Tabs>
      {isAddModalOpen && (
        <AddProduct
          isOpen={isAddModalOpen}
          onClose={() => setIsAddModalOpen(false)}
          productId={0}
          isLoading={false}
        />
      )}
    </div>
  );
}
