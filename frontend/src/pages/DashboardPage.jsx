import { StrictMode, useEffect, useState } from "react"
import InfoCard  from "@/components/ui/InfoCard" 
import { FileText, Package,Plus,ShoppingCart,TriangleAlert,Users2} from "lucide-react"
import PageHeader from "../components/ui/PageHeader"
import Button from "@components/Buttons/Button"
import { fetchWithAuth } from "../services/auth/authService"





export default function DashboardPage () {

const [activeProducts, setActiveProducts] = useState(0);
const [activeCustomers, setActiveCustomers] = useState(0);
const [lowStockProducts, setLowStockProducts] = useState(0);
const [totalSalesOrders, setTotalSalesOrders] = useState(0);



    useEffect(() => {
        console.log("use effect runs ....");
        const fetchData = async () => {
            try{
            const result = await fetchWithAuth("api/dashboard/summary");
        if(!result.success) {
            console.error("Failed to fetch dashboard summary:", result.error);
            throw new Error(result.error);  
        }
        console.log(result);
        
        const { activeProducts, lowStockProducts, activeCustomers, totalSalesOrders } = await result.response.json();
        setActiveProducts(activeProducts);
        setActiveCustomers(activeCustomers);
        setLowStockProducts(lowStockProducts);
        setTotalSalesOrders(totalSalesOrders);
    }catch(error){
        console.error("An error occurred while fetching dashboard data:", error);
    }
    
}
fetchData();
}, [])



    return (
        

            <div className="">
            <div className="flex flex-col md:flex-row md:justify-between md:items-center mb-5">
                <PageHeader title="Dashboard" 
            description="Welcome back! Here's what's happening with your inventory."
            containerClass="sm: mb-4 flex-1"/>
                
                <div className="flex-1 md:justify-end flex">
                <Button variant="secondary" className="  text-sm" LeftIcon={FileText}>Generate Report</Button>
                <Button variant="primary" className="ml-4 text-sm w" LeftIcon={Plus}>Add Product</Button>
                </div>

            </div>
            
            <div className="flex flex-col w-full md:grid md:grid-cols-2 lg:flex lg:flex-row  gap-6 ">
             <InfoCard className="flex-1" title="Total Products" number={activeProducts} description ="active products"
             status={false} statusLabel="-12%" iconComponent={Package} />
             <InfoCard className="flex-1" title="Total Customers" number={activeCustomers} description ="active customers"
             status={true} statusLabel="+12%" iconComponent={Users2} />
            <InfoCard className="flex-1" title="Low Stock Items" number={lowStockProducts} description ="need restock"
             status={false} statusLabel="-4%" iconComponent={TriangleAlert} />
            <InfoCard className="flex-1" title="Total Sales Orders" number={totalSalesOrders} description ="sales last month"
              status={true} statusLabel="+8%" iconComponent={ShoppingCart} />
            </div>
            




            </div>
            
            
      
       
    )
}