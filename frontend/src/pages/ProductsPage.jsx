import { useState, useEffect } from 'react';
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
import { getSummary } from '@services/products/productService';
import DataTable from '@components/DataTable/DataTable';
import { Tab, Tabs, TabList, TabPanel } from 'react-tabs';
import { divStyles } from '../util/uiVariables';
import ProductDataTable from '../components/ui/ProductsTables/ProductDataTable';
import StockMovementHistoryTable from '../components/ui/ProductsTables/StockMovementHistoryTable';
import { AddProduct } from '@components/products';
import UnitOfMeasureTable from '@/components/unitOfMeasure/UnitOfMeasureTable';
export default function ProductsPage() {
  const [totalProductsCount, setTotalProductsCount] = useState(0);
  const [inventoryValue, setInventoryValue] = useState(0);
  const [lowStockCount, setLowStockCount] = useState(1);
  const [profitPotential, setProfitPotential] = useState(1);
  const [loading, setLoading] = useState(false);
  const [isAddModalOpen, setIsAddModalOpen] = useState(false);

  useEffect(() => {
    const fetchData = async () => {
      setLoading(true);
      const summary = await getSummary();
      setTotalProductsCount(summary.totalProducts);
      setInventoryValue(summary.inventoryValue);
      setLowStockCount(summary.lowStockProducts);
      setProfitPotential(summary.profitPotential);
    };
    fetchData();
    setLoading(false);
  }, []);

  return (
    <div>
      <div className='flex flex-col md:flex-row md:items-center md:justify-between mb-6 gap-4'>
        <PageHeader
          title='Product Management'
          description='Manage your product catalog and inventory.'
        />

        <Button LeftIcon={Plus} onClick={() => setIsAddModalOpen(true)}>
          Add Product
        </Button>
      </div>
      <div className='flex flex-col md:flex-row gap-6'>
        <InfoCard
          title='Total Products'
          iconComponent={Package}
          number={loading ? '...' : totalProductsCount.toLocaleString()}
          description='Easily add new products to your catalog.'
          className='flex-1'
        />
        <InfoCard
          title='Inventory Value'
          iconComponent={DollarSign}
          number={
            loading
              ? '...'
              : `$${inventoryValue.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`
          }
          description='Total value of all products in inventory.'
          className='flex-1'
          numberClassName='text-green-600'
          iconClassName='text-green-600'
        />
        <InfoCard
          title='Low Stock Items'
          description='Products that need restocking soon.'
          number={loading ? '...' : lowStockCount}
          iconComponent={TriangleAlert}
          className='flex-1'
          numberClassName={lowStockCount > 0 ? 'text-red-600' : ''}
          iconClassName={lowStockCount > 0 ? 'text-red-600' : ''}
        />
        <InfoCard
          title='Profit Potential'
          description='if all products are sold at retail price.'
          number={
            loading
              ? '...'
              : `$${profitPotential.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`
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
            Products
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
            Unit Of Measure
          </Tab>
          <Tab
            className='px-4 py-2 rounded-md cursor-pointer text-gray-600 transition-colors hover:text-gray-800'
            selectedClassName='!bg-white !text-gray-900 shadow-sm'
          >
            Product Categories
          </Tab>
          <Tab
            className='px-4 py-2 rounded-md cursor-pointer text-gray-600 transition-colors hover:text-gray-800'
            selectedClassName='!bg-white !text-gray-900 shadow-sm'
          >
            Product Images
          </Tab>
        </TabList>
        <div className={divStyles + 'mt-6'}>
          <TabPanel>
            <div className='mb-9 flex items-center justify-between'>
              <h3 className='text-2xl font-semibold leading-none tracking-tight'>
                Product Catalog
              </h3>
            </div>
            <ProductDataTable />
          </TabPanel>
          <TabPanel>
            <div className='mb-9 flex items-center justify-between'>
              <h3 className='text-2xl font-semibold leading-none tracking-tight'>
                Stock Movement History
              </h3>
            </div>
            <StockMovementHistoryTable />
          </TabPanel>
          <TabPanel>
            <div className='mb-9 flex items-center justify-between'>
              <h3 className='text-2xl font-semibold leading-none tracking-tight'>
                Unit Of Measure
              </h3>
            </div>
            <UnitOfMeasureTable />
          </TabPanel>
          <TabPanel>
            <div className='mb-9 flex items-center justify-between'>
              <h3 className='text-2xl font-semibold leading-none tracking-tight'>
                Product Categories
              </h3>
            </div>
            <div> Product Categories Will be here !</div>
          </TabPanel>
          <TabPanel>
            <div className='mb-9 flex items-center justify-between'>
              <h3 className='text-2xl font-semibold leading-none tracking-tight'>
                Product Images
              </h3>
            </div>
            <div> Product Images Will be here !</div>
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
