import React from 'react';
import { useQuery } from '@tanstack/react-query';
import { useTranslation } from 'react-i18next';

import i18nKeyContainer from '@shared/lib/i18n/keyContainer';
import TopProductItem from './TopProductItem';
import dashboardApi from '@features/dashboard/services/dashboardApi';
import { queryKeys } from '@shared/lib/queryKeys';
import { formatDzdCurrency } from '@shared/utils/currencyFormatter';

const TopSellingProducts = ({ className = '' }) => {
  const { t, i18n } = useTranslation();
  const activeLocale = i18n.resolvedLanguage || i18n.language || 'en';

  const { data: products = [], isLoading } = useQuery({
    queryKey: queryKeys.dashboard.topSellingProducts(7),
    queryFn: () => dashboardApi.getTopSellingProducts(7),
  });

  if (isLoading) {
    return (
      <div className={`p-4 border border-gray-200 rounded-lg ${className}`}>
        {t(i18nKeyContainer.dashboard.topProducts.loading)}
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
          revenue={formatDzdCurrency(product.totalRevenue, {
            locale: activeLocale,
          })}
        />
      ))}
    </div>
  );
};

export default TopSellingProducts;
