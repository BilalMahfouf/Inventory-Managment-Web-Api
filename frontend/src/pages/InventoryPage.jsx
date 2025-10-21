import { useState, useEffect } from 'react';
import InfoCard from '@/components/ui/InfoCard';
import PageHeader from '@components/ui/PageHeader';
import Button from '@components/Buttons/Button';

import { AlertTriangle, Boxes, Package, Plus, XCircle } from 'lucide-react';
import { divStyles } from '@/util/uiVariables';
import { TabList, Tabs, Tab, TabPanel } from 'react-tabs';
import InventoryDataTable from '@/components/inventory/InventoryDataTable';
import AddUpdateInventory from '@/components/inventory/AddUpdateInventory';
import { getInventorySummary } from '@/services/inventoryService';

export default function InventoryPage() {
  const [loading, setLoading] = useState(false);
  const [totalInventoryItems, setTotalInventoryItems] = useState(0);
  const [totalPotentialProfit, setTotalPotentialProfit] = useState(0);
  const [lowStockCount, setLowStockCount] = useState(0);
  const [outOfStockCount, setOutOfStockCount] = useState(0);
  const [isAddModalOpen, setIsAddModalOpen] = useState(false);

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
          title='Inventory Management'
          description='Manage your product catalog and inventory.'
        />

        <Button LeftIcon={Plus} onClick={() => setIsAddModalOpen(true)}>
          Add Inventory
        </Button>
      </div>
      <div className='flex flex-col md:flex-row gap-6'>
        <InfoCard
          title='Total Inventory Items'
          iconComponent={Package}
          number={loading ? '...' : totalInventoryItems.toLocaleString()}
          description='Easily add new products to your catalog.'
          className='flex-1'
        />
        <InfoCard
          title='Total Potential Profit'
          iconComponent={Boxes}
          number={loading ? '...' : `$${totalPotentialProfit.toLocaleString()}`}
          description='Total potential profit from all products in stock.'
          className='flex-1'
        />
        <InfoCard
          title='Low Stock Items'
          iconComponent={AlertTriangle}
          number={loading ? '...' : lowStockCount}
          description='Products that are low in stock.'
          className='flex-1'
        />
        <InfoCard
          title='Out of Stock Items'
          iconComponent={XCircle}
          number={loading ? '...' : outOfStockCount}
          description='Products that are out of stock.'
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
              Inventory
            </Tab>
            <Tab
              className='px-4 py-2 rounded-md cursor-pointer text-gray-600 transition-colors hover:text-gray-800'
              selectedClassName='!bg-white !text-gray-900 shadow-sm'
            >
              Stock Movements
            </Tab>
            <Tab
              className='px-4 py-2 rounded-md cursor-pointer text-gray-600 transition-colors hover:text-gray-800'
              selectedClassName='!bg-white !text-gray-900 shadow-sm'
            >
              Stock Transfers
            </Tab>
            <Tab
              className='px-4 py-2 rounded-md cursor-pointer text-gray-600 transition-colors hover:text-gray-800'
              selectedClassName='!bg-white !text-gray-900 shadow-sm'
            >
              Stock Alerts
            </Tab>
          </TabList>
          <div className={divStyles + 'mt-6'}>
            <TabPanel>
              <div className='mb-9 flex items-center justify-between'>
                <h3 className='text-2xl font-semibold leading-none tracking-tight'>
                  Inventory Catalog
                </h3>
              </div>
              <InventoryDataTable />
            </TabPanel>
            <TabPanel>
              <div className='mb-9 flex items-center justify-between'>
                <h3 className='text-2xl font-semibold leading-none tracking-tight'>
                  Stock Movements
                </h3>
              </div>
              <div>Stock Movements will be here !</div>
            </TabPanel>
            <TabPanel>
              <div className='mb-9 flex items-center justify-between'>
                <h3 className='text-2xl font-semibold leading-none tracking-tight'>
                  Stock Transfers
                </h3>
                <div>Stock transfers will be here !</div>
              </div>
            </TabPanel>
            <TabPanel>
              <div className='mb-9 flex items-center justify-between'>
                <h3 className='text-2xl font-semibold leading-none tracking-tight'>
                  Stock Alerts
                </h3>
                <div>Stock alerts will be here !</div>
              </div>
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
