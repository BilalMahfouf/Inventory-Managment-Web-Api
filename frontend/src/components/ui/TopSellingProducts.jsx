import React, { useEffect, useState } from 'react';
import TopProductItem from './TopProductItem';
import dashboardService from '@services/dashboardService';

const TopSellingProducts = ({ className = '' }) => {
  // Sample data if no products provided
  const [products, setProducts] = useState([]);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const fetchTopProducts = async () => {
      setIsLoading(true);
      const products = await dashboardService.getTopSellingProducts(7);
      if (products && products.length > 0) {
        setProducts(products);
        setIsLoading(false);
        return;
      }
      setProducts([]);
    }
    fetchTopProducts();
  }, []);
  if (isLoading) {
    return (
      <div className={`p-4 border border-gray-200 rounded-lg ${className}`}>
        Loading...
      </div>
    );
  }
  return (
    <div className={`space-y-2 ${className}`}>
      {products.map((product, index) => (
        <TopProductItem
          key={product.index}
          rank={product.rank || index + 1}
          productName={product.name}
          unitsSold={product.totalSoldUnits}
          revenue={`$${product.totalRevenue}`}
        />
      ))}
    </div>
  );
};

export default TopSellingProducts;
