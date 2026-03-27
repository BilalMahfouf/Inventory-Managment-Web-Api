import React, { useState, useEffect } from 'react';
import { fetchWithAuth } from '../../services/auth/authService';
import TodaysPerformance from './TodaysPerformance';

const TodaysPerformanceContainer = ({ className = '' }) => {
  const [performanceData, setPerformanceData] = useState({
    todaysSales: '$0',
    salesChange: '+0%',
    newOrders: '0',
    ordersChange: '+0%',
    newCustomers: '0',
    customersChange: '+0%',
    productsSold: '0',
    productsSoldChange: '+0%',
  });
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchTodaysPerformance = async () => {
      try {
        setLoading(true);
        setError(null);

        // Replace with your actual API endpoint
        const result = await fetchWithAuth('api/dashboard/today-performance');

        if (!result.success) {
          throw new Error(
            result.error || "Failed to fetch today's performance data"
          );
        }

        const data = await result.response.json();

        // Map the API response to component props
        // Adjust these field names based on your actual API response structure
        setPerformanceData({
          todaysSales: data.todayRevenues || '$0',
          salesChange: data.salesChange || '+0%',
          newOrders: data.todayNewOrders?.toString() || '0',
          ordersChange: data.ordersChange || '+0%',
          newCustomers: data.todayNewCustomers?.toString() || '0',
          customersChange: data.customersChange || '+0%',
          productsSold: data.todaySoldProducts?.toString() || '0',
          productsSoldChange: data.productsSoldChange || '+0%',
        });
      } catch (err) {
        console.error("Error fetching today's performance:", err);
        setError(err.message);

        // Set sample data for development/fallback
        setPerformanceData({
          todaysSales: '$12,450',
          salesChange: '+8.2%',
          newOrders: '23',
          ordersChange: '+12%',
          newCustomers: '8',
          customersChange: '+5%',
          productsSold: '156',
          productsSoldChange: '+15%',
        });
      } finally {
        setLoading(false);
      }
    };

    fetchTodaysPerformance();
  }, []);

  // You can handle error state here if needed
  if (error) {
    console.warn('Using fallback data due to error:', error);
  }

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
      loading={loading}
      className={className}
    />
  );
};

export default TodaysPerformanceContainer;
