import { createBrowserRouter, RouterProvider } from 'react-router-dom';
import { Login } from '@features/auth/pages/Login';
import NotFoundPage from '@shared/pages/NotFoundPage';
import DashboardPage from '@features/dashboard/pages/DashboardPage';
import ProductsPage from '@features/products/pages/ProductsPage';
import SalesPage from '@features/sales/pages/SalesPage';
import CustomersPage from '@features/customers/pages/CustomersPage';
import SettingsPage from '@features/settings/pages/SettingsPage';
import MainLayout from '@components/layout/MainLayout';
import InventoryPage from '@features/inventory/pages/InventoryPage';
import { ToastProvider } from '@shared/context/ToastContext';

const router = createBrowserRouter([
  {
    path: '/',
    element: <Login />,
    errorElement: <NotFoundPage />,
  },
  {
    element: <MainLayout />, // wrap all pages with sidebar
    errorElement: <NotFoundPage />,
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
]);

function App() {
  return (
    <ToastProvider>
      <RouterProvider router={router} />
    </ToastProvider>
  );
}

export default App;
