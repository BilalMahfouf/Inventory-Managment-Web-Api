import { createBrowserRouter, RouterProvider } from 'react-router-dom';
import { Login } from '@/pages/Login';
import NotFoundPage from '@/pages/NotFoundPage';
import DashboardPage from '@/pages/DashboardPage';
import ProductsPage from '@/pages/ProductsPage';
import InventoryPage from '@/pages/InventoryPage';
import SalesPage from '@/pages/SalesPage';
import CustomersPage from '@/pages/CustomersPage';
import SettingsPage from '@/pages/SettingsPage';
import MainLayout from '@/layout/MainLayout';
import { ToastProvider } from '@/context/ToastContext';

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
