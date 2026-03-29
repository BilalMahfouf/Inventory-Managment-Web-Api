import React from 'react';
import { useQuery } from '@tanstack/react-query';
import dashboardApi from '@features/dashboard/services/dashboardApi';
import TodaysPerformance from './TodaysPerformance';
import { queryKeys } from '@shared/lib/queryKeys';

const fallbackPerformanceData = {
  todaysSales: '$12,450',
  salesChange: '+8.2%',
  newOrders: '23',
  ordersChange: '+12%',
  newCustomers: '8',
  customersChange: '+5%',
  productsSold: '156',
  productsSoldChange: '+15%',
};

const TodaysPerformanceContainer = ({ className = '' }) => {
  const { data, isLoading, isError } = useQuery({
    queryKey: queryKeys.dashboard.todayPerformance(),
    queryFn: dashboardApi.getTodayPerformance,
  });

  const performanceData = isError
    ? fallbackPerformanceData
    : {
        todaysSales: data?.todayRevenues || '$0',
        salesChange: data?.salesChange || '+0%',
        newOrders: data?.todayNewOrders?.toString() || '0',
        ordersChange: data?.ordersChange || '+0%',
        newCustomers: data?.todayNewCustomers?.toString() || '0',
        customersChange: data?.customersChange || '+0%',
        productsSold: data?.todaySoldProducts?.toString() || '0',
        productsSoldChange: data?.productsSoldChange || '+0%',
      };

  return (
    <TodaysPerformance
      todaysSales={performanceData.todaysSales}
      salesChange={performanceData.salesChange}
      newOrders={performanceData.newOrders}
      ordersChange={performanceData.ordersChange}
      newCustomers={performanceData.newCustomers}
      customersChange={performanceData.customersChange}
      productsSold={performanceData.productsSold}
      productsSoldChange={performanceData.productsSoldChange}
      loading={isLoading}
      className={className}
    />
  );
};

export default TodaysPerformanceContainer;
