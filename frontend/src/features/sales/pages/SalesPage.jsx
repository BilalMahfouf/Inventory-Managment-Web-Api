import { useState, useEffect } from 'react';
import { getOrdersDahsobardSummary } from '@features/sales/services/salesOrderApi';
import InfoCard from '@/components/ui/InfoCard';

import { TabList, Tabs, Tab, TabPanel } from 'react-tabs';
import PageHeader from '@/components/ui/PageHeader';
import Button from '@/components/Buttons/Button';
import { AlertTriangle, Boxes, Package, Plus, XCircle } from 'lucide-react';
import { divStyles } from '@shared/utils/uiVariables';

export default function SalesPage() {
  const [totalOrders, setTotalOrders] = useState(null);
  const [pendingOrders, setPendingOrders] = useState(null);
  const [averageOrderValue, setAverageOrderValue] = useState(null);
  const [revenueThisMonth, setRevenueThisMonth] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const getDashboardData = async () => {
      setLoading(true);
      const result = await getOrdersDahsobardSummary();
      if (!result.isSuccess) {
        return;
      }
      setTotalOrders(result.data.totalOrders);
      setAverageOrderValue(result.data.averageOrderValue);
      setPendingOrders(result.data.pendingOrders);
      setRevenueThisMonth(result.data.revenueThisMonth);
      setLoading(false);
    };
    getDashboardData();
  }, []);

  return (
    <>
      <div className='flex flex-col md:flex-row md:items-center md:justify-between mb-6 gap-4'>
        <PageHeader
          title='Inventory Management'
          description='Manage your product catalog and inventory.'
        />

        <Button LeftIcon={Plus} onClick={() => {}}>
          Add Inventory
        </Button>
      </div>
      <div className='flex flex-col md:flex-row gap-6'>
        <InfoCard
          title='Total Orders'
          iconComponent={Package}
          number={loading ? '...' : totalOrders}
          description=''
          className='flex-1'
        />
        <InfoCard
          title='Pending Orders'
          iconComponent={Boxes}
          number={loading ? '...' : `${pendingOrders}`}
          description='Total potential profit from all products in stock.'
          className='flex-1'
        />
        <InfoCard
          title='Avg Order Value'
          iconComponent={AlertTriangle}
          number={loading ? '...' : averageOrderValue}
          description=''
          className='flex-1'
        />
        <InfoCard
          title='Revenue'
          iconComponent={XCircle}
          number={loading ? '...' : `$${revenueThisMonth}`}
          description='Revenue this month'
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
            <Tab
              className='px-4 py-2 rounded-md cursor-pointer text-gray-600 transition-colors hover:text-gray-800'
              selectedClassName='!bg-white !text-gray-900 shadow-sm'
            >
              Locations
            </Tab>
          </TabList>
          <div className={divStyles + 'mt-6'}>
            <TabPanel>
              <div className='mb-9 flex items-center justify-between'>
                <h3 className='text-2xl font-semibold leading-none tracking-tight'>
                  Inventory Catalog
                </h3>
              </div>
              {/* <InventoryDataTable /> */}
            </TabPanel>
            <TabPanel>
              <div className='mb-9 flex items-center justify-between'>
                <h3 className='text-2xl font-semibold leading-none tracking-tight'>
                  Stock Movements
                </h3>
              </div>
              {/* <StockMovementHistoryTable /> */}
            </TabPanel>
            <TabPanel>
              <div className='mb-9 flex items-center justify-between'>
                <h3 className='text-2xl font-semibold leading-none tracking-tight'>
                  Stock Transfers
                </h3>
                {/* <AddStockTransferButton /> */}
              </div>
              {/* <StockTransferDataTable /> */}
            </TabPanel>
            <TabPanel>
              <div className='mb-9 flex items-center justify-between'>
                <h3 className='text-2xl font-semibold leading-none tracking-tight'>
                  Stock Alerts
                </h3>
                <div>Stock alerts will be here !</div>
              </div>
            </TabPanel>
            <TabPanel>
              <div className='mb-9 flex items-center justify-between'>
                <h3 className='text-2xl font-semibold leading-none tracking-tight'>
                  Locations
                </h3>
                {/* <AddLocationButton /> */}
              </div>
              {/* <LocationDataTable /> */}
            </TabPanel>
          </div>
        </Tabs>
      </div>
    </>
  );
}
