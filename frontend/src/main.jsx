import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import { createBrowserRouter, RouterProvider, Outlet } from "react-router-dom";

import "./index.css";
import { Login } from "@/pages/Login";
import NotFoundPage from "./pages/NotFoundPage";
import DashboardPage from "./pages/DashboardPage";
import Sidebar from "@components/ui/sideBar/Sidebar";
import ProductsPage from "./pages/ProductsPage";
import InventoryPage from "./pages/InventoryPage";
import SalesPage from "./pages/SalesPage";
import CustomersPage from "./pages/CustomersPage";
import SettingsPage from "./pages/SettingsPage";

// Layout with sidebar
function MainLayout() {
  return (
    <div className="flex">
      <Sidebar />
      <main className="flex-1 p-4">
        <Outlet /> {/* This is where child routes render */}
      </main>
    </div>
  );
}

const router = createBrowserRouter([
  
    {
        path: "/",
        element:<Login />,
        errorElement: <NotFoundPage />
    },
    {
    element: <MainLayout />, // wrap all pages with sidebar
    errorElement: <NotFoundPage />,
    children: [
      {
        path: "/dashboard",
        element: <DashboardPage />,
      },
       {
        path: "/products",
        element: <ProductsPage />,
      }, {
        path: "/inventory",
        element: <InventoryPage />,
      }, {
        path: "/sales",
        element: <SalesPage />,
      }, {
        path: "/customers",
        element: <CustomersPage />,
      },
       {
        path: "/settings",
        element: <SettingsPage />,
      },
    ],
}
]);

createRoot(document.getElementById("root")).render(
  <StrictMode>
    <RouterProvider router={router} />
  </StrictMode>
);
