import { createBrowserRouter, Navigate } from 'react-router-dom';
import MainLayout from '@components/layout/MainLayout';
import { Login } from '@features/auth/pages/Login';
import AuthGuard from '@features/auth/AuthGuard';
import ForgotPasswordPage from '@features/auth/pages/ForgotPasswordPage';
import DashboardPage from '@features/dashboard/pages/DashboardPage';
import ProductsPage from '@features/products/pages/ProductsPage';
import InventoryPage from '@features/inventory/pages/InventoryPage';
import SalesPage from '@features/sales/pages/SalesPage';
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
    element: <Navigate to="/" replace />,
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
            element: <SalesPage />,
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
