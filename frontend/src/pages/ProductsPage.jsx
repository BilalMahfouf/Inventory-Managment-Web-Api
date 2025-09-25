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
export default function ProductsPage() {
  const [totalProductsCount, setTotalProductsCount] = useState(0);
  const [inventoryValue, setInventoryValue] = useState(0);
  const [lowStockCount, setLowStockCount] = useState(1);
  const [profitPotential, setProfitPotential] = useState(1);
  const [loading, setLoading] = useState(false);

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
        <Button LeftIcon={Plus} children='Add Product' />
      </div>
      <div className='flex flex-col md:flex-row gap-6'>
        <InfoCard
          title='Total Products'
          iconComponent={Package}
          number={loading ? '...' : totalProductsCount}
          description='Easily add new products to your catalog.'
          className='flex-1'
        />
        <InfoCard
          title='Inventory Value'
          iconComponent={DollarSign}
          number={loading ? '...' : `$${inventoryValue}`}
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
          number={loading ? '...' : `$${profitPotential}`}
          iconComponent={TrendingUp}
          className='flex-1'
        />
      </div>
    </div>
  );
}
