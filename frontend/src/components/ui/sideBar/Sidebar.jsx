import Logo from "@components/ui/Logo"
import SideBarLink from "./SideBarLink"
import { Package,Warehouse ,TrendingUp,Users,Settings,LayoutPanelLeft} from "lucide-react"




export default function SideBar() {


    return (
        <div className="hidden lg:block p-4 bg-gray-900 h-screen w-3xs">
            <div className="flex items-start gap-2   ">
                <Logo extraStye="" />
                <span className="text-white font-bold text-[20px] mr-8.5 mt-1 ">InventoryPro</span>
            </div>
            <div className="flex gap-3 flex-col mt-4">
                <SideBarLink pathname="/dashboard"content="Dashboard" leftIcon={LayoutPanelLeft} />
                <SideBarLink pathname="/products"content="Products" leftIcon={Package} />
                <SideBarLink pathname="/inventory"content="Inventory"leftIcon={Warehouse} />
                <SideBarLink pathname="/sales"content="Sales"leftIcon={TrendingUp} />
                <SideBarLink pathname="/customers"content="Customers"leftIcon={Users} />
                <SideBarLink pathname="/settings"content="Settings" leftIcon={Settings}/>
            </div>
        </div>
    )

}