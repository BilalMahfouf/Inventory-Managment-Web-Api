import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import {createBrowserRouter,RouterProvider} from "react-router-dom";



import './index.css'
import {Login} from "@/pages/Login";
import NotFoundPage from './pages/NotFoundPage';
import DashboardPage from './pages/DashboardPage';

const router = createBrowserRouter([{
    path:"/",
    element:<Login />,
    errorElement: <NotFoundPage />
},
{
    path:"/dashboard",
    element: <DashboardPage />
}


]);

createRoot(document.getElementById('root')).render(
  <StrictMode>
    <RouterProvider router={router} />
  </StrictMode>,
)
