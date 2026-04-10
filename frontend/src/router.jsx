import { createBrowserRouter, Navigate } from 'react-router-dom';
import MainLayout from '@components/layout/MainLayout';
import { Login } from '@features/auth/pages/Login';
import AuthGuard from '@features/auth/AuthGuard';
import ForgotPasswordPage from '@features/auth/pages/ForgotPasswordPage';
import DashboardPage from '@features/dashboard/pages/DashboardPage';
import ProductsPage from '@features/products/pages/ProductsPage';
import InventoryPage from '@features/inventory/pages/InventoryPage';
import OrdersPage from '@features/sales/pages/OrdersPage';
import CreateOrderPage from '@features/sales/pages/CreateOrderPage';
import OrderDetailPage from '@features/sales/pages/OrderDetailPage';
import EditOrderPage from '@features/sales/pages/EditOrderPage';
import CustomersPage from '@features/customers/pages/CustomersPage';
import SettingsPage from '@features/settings/pages/SettingsPage';
import NotFoundPage from '@shared/pages/NotFoundPage';

export const router = createBrowserRouter([
  {
    path: '/',
    element: <Login />,
  },
  {
    path: '/login',
    element: <Navigate to='/' replace />,
  },
  {
    path: '/forgot-password',
    element: <ForgotPasswordPage />,
  },
  {
    element: <AuthGuard />,
    children: [
      {
        element: <MainLayout />,
        children: [
          {
            path: '/dashboard',
            element: <DashboardPage />,
          },
          {
            path: '/products',
            element: <ProductsPage />,
          },
          {
            path: '/inventory',
            element: <InventoryPage />,
          },
          {
            path: '/sales',
            element: <Navigate to='/sales-orders' replace />,
          },
          {
            path: '/orders',
            element: <Navigate to='/sales-orders' replace />,
          },
          {
            path: '/sales-orders',
            element: <OrdersPage />,
          },
          {
            path: '/sales-orders/new',
            element: <CreateOrderPage />,
          },
          {
            path: '/sales-orders/:id',
            element: <OrderDetailPage />,
          },
          {
            path: '/sales-orders/:id/edit',
            element: <EditOrderPage />,
          },
          {
            path: '/orders/new',
            element: <Navigate to='/sales-orders/new' replace />,
          },
          {
            path: '/orders/:id',
            element: <OrderDetailPage />,
          },
          {
            path: '/orders/:id/edit',
            element: <EditOrderPage />,
          },
          {
            path: '/customers',
            element: <CustomersPage />,
          },
          {
            path: '/settings',
            element: <SettingsPage />,
          },
        ],
      },
    ],
  },
  {
    path: '*',
    element: <NotFoundPage />,
  },
]);
