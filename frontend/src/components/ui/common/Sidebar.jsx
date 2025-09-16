import Logo from "@components/ui/Logo"
import SideBarLink from "./SideBarLink"
import { Package, Warehouse, TrendingUp, Users, Settings, LayoutPanelLeft } from "lucide-react"

export default function SideBar() {
    return (
        <div className="hidden lg:block fixed left-0 top-16 bottom-0 w-64 bg-gray-900 z-40">
            <div className="p-4 h-full overflow-y-auto">
                <div className="flex items-start gap-2">
                    <Logo extraStye="" />
                    <span className="text-white font-bold text-[20px] mr-8.5 mt-1">InventoryPro</span>
                </div>
                <div className="flex gap-3 flex-col mt-6">
                    <SideBarLink pathname="/dashboard" content="Dashboard" leftIcon={LayoutPanelLeft} />
                    <SideBarLink pathname="/products" content="Products" leftIcon={Package} />
                    <SideBarLink pathname="/inventory" content="Inventory" leftIcon={Warehouse} />
                    <SideBarLink pathname="/sales" content="Sales" leftIcon={TrendingUp} />
                    <SideBarLink pathname="/customers" content="Customers" leftIcon={Users} />
                    <SideBarLink pathname="/settings" content="Settings" leftIcon={Settings} />
                </div>
            </div>
        </div>
    )
}