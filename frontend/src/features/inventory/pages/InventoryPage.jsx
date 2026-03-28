import { useState, useEffect } from 'react';
import InfoCard from '@/components/ui/InfoCard';
import PageHeader from '@components/ui/PageHeader';
import Button from '@components/Buttons/Button';

import { AlertTriangle, Boxes, Package, Plus, XCircle } from 'lucide-react';
import { divStyles } from '@shared/utils/uiVariables';
import { TabList, Tabs, Tab, TabPanel } from 'react-tabs';
import InventoryDataTable from '@features/inventory/components/inventory/InventoryDataTable';
import AddUpdateInventory from '@features/inventory/components/inventory/AddUpdateInventory';
import { getInventorySummary } from '@features/inventory/services/inventoryApi';
import StockTransferDataTable from '@features/inventory/components/stockTransfers/StockTransferDataTable';
import AddStockTransferButton from '@features/inventory/components/stockTransfers/AddStockTransferButton';
import LocationDataTable from '@features/inventory/components/locations/LocationDataTable';
import AddLocationButton from '@features/inventory/components/locations/AddLocationButton';
import StockMovementHistoryTable from '@features/products/components/ProductsTables/StockMovementHistoryTable';
import { useTranslation } from 'react-i18next';
import i18nKeyContainer from '@shared/lib/i18n/keyContainer';

export default function InventoryPage() {
  const { t, i18n } = useTranslation();
  const [loading, setLoading] = useState(false);
  const [totalInventoryItems, setTotalInventoryItems] = useState(0);
  const [totalPotentialProfit, setTotalPotentialProfit] = useState(0);
  const [lowStockCount, setLowStockCount] = useState(0);
  const [outOfStockCount, setOutOfStockCount] = useState(0);
  const [isAddModalOpen, setIsAddModalOpen] = useState(false);
  const activeLocale = i18n.resolvedLanguage || i18n.language || 'en';

  const handleSummaryData = async () => {
    setLoading(true);
    const response = await getInventorySummary();
    if (response.success) {
      setTotalInventoryItems(response.data.totalInventoryItems);
      setTotalPotentialProfit(response.data.totalPotentialProfit);
      setLowStockCount(response.data.lowStockItems);
      setOutOfStockCount(response.data.outOfStockItems);
      setLoading(false);
      return;
    }
    setLoading(false);
  };
  useEffect(() => {
    handleSummaryData();
  }, []);

  return (
    <>
      <div className='flex flex-col md:flex-row md:items-center md:justify-between mb-6 gap-4'>
        <PageHeader
          title={t(i18nKeyContainer.inventory.page.title)}
          description={t(i18nKeyContainer.inventory.page.description)}
        />

        <Button LeftIcon={Plus} onClick={() => setIsAddModalOpen(true)}>
          {t(i18nKeyContainer.inventory.page.addInventory)}
        </Button>
      </div>
      <div className='flex flex-col md:flex-row gap-6'>
        <InfoCard
          title={t(i18nKeyContainer.inventory.cards.totalInventoryItems.title)}
          iconComponent={Package}
          number={loading ? '...' : totalInventoryItems.toLocaleString(activeLocale)}
          description={t(
            i18nKeyContainer.inventory.cards.totalInventoryItems.description
          )}
          className='flex-1'
        />
        <InfoCard
          title={t(i18nKeyContainer.inventory.cards.totalPotentialProfit.title)}
          iconComponent={Boxes}
          number={
            loading
              ? '...'
              : `$${totalPotentialProfit.toLocaleString(activeLocale, {
                  minimumFractionDigits: 2,
                  maximumFractionDigits: 2,
                })}`
          }
          description={t(
            i18nKeyContainer.inventory.cards.totalPotentialProfit.description
          )}
          className='flex-1'
        />
        <InfoCard
          title={t(i18nKeyContainer.inventory.cards.lowStockItems.title)}
          iconComponent={AlertTriangle}
          number={loading ? '...' : lowStockCount.toLocaleString(activeLocale)}
          description={t(i18nKeyContainer.inventory.cards.lowStockItems.description)}
          className='flex-1'
        />
        <InfoCard
          title={t(i18nKeyContainer.inventory.cards.outOfStockItems.title)}
          iconComponent={XCircle}
          number={loading ? '...' : outOfStockCount.toLocaleString(activeLocale)}
          description={t(
            i18nKeyContainer.inventory.cards.outOfStockItems.description
          )}
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
              {t(i18nKeyContainer.inventory.page.tabs.inventory)}
            </Tab>
            <Tab
              className='px-4 py-2 rounded-md cursor-pointer text-gray-600 transition-colors hover:text-gray-800'
              selectedClassName='!bg-white !text-gray-900 shadow-sm'
            >
              {t(i18nKeyContainer.inventory.page.tabs.stockMovements)}
            </Tab>
            <Tab
              className='px-4 py-2 rounded-md cursor-pointer text-gray-600 transition-colors hover:text-gray-800'
              selectedClassName='!bg-white !text-gray-900 shadow-sm'
            >
              {t(i18nKeyContainer.inventory.page.tabs.stockTransfers)}
            </Tab>
            <Tab
              className='px-4 py-2 rounded-md cursor-pointer text-gray-600 transition-colors hover:text-gray-800'
              selectedClassName='!bg-white !text-gray-900 shadow-sm'
            >
              {t(i18nKeyContainer.inventory.page.tabs.stockAlerts)}
            </Tab>
            <Tab
              className='px-4 py-2 rounded-md cursor-pointer text-gray-600 transition-colors hover:text-gray-800'
              selectedClassName='!bg-white !text-gray-900 shadow-sm'
            >
              {t(i18nKeyContainer.inventory.page.tabs.locations)}
            </Tab>
          </TabList>
          <div className={divStyles + 'mt-6'}>
            <TabPanel>
              <div className='mb-9 flex items-center justify-between'>
                <h3 className='text-2xl font-semibold leading-none tracking-tight'>
                  {t(i18nKeyContainer.inventory.page.sections.inventoryCatalog)}
                </h3>
              </div>
              <InventoryDataTable />
            </TabPanel>
            <TabPanel>
              <div className='mb-9 flex items-center justify-between'>
                <h3 className='text-2xl font-semibold leading-none tracking-tight'>
                  {t(i18nKeyContainer.inventory.page.sections.stockMovements)}
                </h3>
              </div>
              <StockMovementHistoryTable />
            </TabPanel>
            <TabPanel>
              <div className='mb-9 flex items-center justify-between'>
                <h3 className='text-2xl font-semibold leading-none tracking-tight'>
                  {t(i18nKeyContainer.inventory.page.sections.stockTransfers)}
                </h3>
                <AddStockTransferButton />
              </div>
              <StockTransferDataTable />
            </TabPanel>
            <TabPanel>
              <div className='mb-9 flex items-center justify-between'>
                <h3 className='text-2xl font-semibold leading-none tracking-tight'>
                  {t(i18nKeyContainer.inventory.page.sections.stockAlerts)}
                </h3>
                <div>{t(i18nKeyContainer.inventory.page.placeholders.stockAlerts)}</div>
              </div>
            </TabPanel>
            <TabPanel>
              <div className='mb-9 flex items-center justify-between'>
                <h3 className='text-2xl font-semibold leading-none tracking-tight'>
                  {t(i18nKeyContainer.inventory.page.sections.locations)}
                </h3>
                <AddLocationButton />
              </div>
              <LocationDataTable />
            </TabPanel>
          </div>
        </Tabs>
      </div>
      {isAddModalOpen && (
        <AddUpdateInventory
          isOpen={isAddModalOpen}
          onClose={() => setIsAddModalOpen(false)}
        />
      )}
    </>
  );
}
