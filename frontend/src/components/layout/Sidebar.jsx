import Logo from "@/components/ui/Logo"
import SideBarLink from "@/components/ui/common/SideBarLink"
import { Package, Warehouse, TrendingUp, Users, Settings, LayoutPanelLeft, X } from "lucide-react"
import { useTranslation } from "react-i18next"
import i18nKeyContainer from "@shared/lib/i18n/keyContainer"

export default function Sidebar({ isOpen, onClose }) {
    const { t } = useTranslation();

    const menuItems = [
        { pathname: "/dashboard", contentKey: i18nKeyContainer.layout.sidebar.menu.dashboard, icon: LayoutPanelLeft },
        { pathname: "/products", contentKey: i18nKeyContainer.layout.sidebar.menu.products, icon: Package },
        { pathname: "/inventory", contentKey: i18nKeyContainer.layout.sidebar.menu.inventory, icon: Warehouse },
        { pathname: "/sales", contentKey: i18nKeyContainer.layout.sidebar.menu.sales, icon: TrendingUp },
        { pathname: "/customers", contentKey: i18nKeyContainer.layout.sidebar.menu.customers, icon: Users },
        { pathname: "/settings", contentKey: i18nKeyContainer.layout.sidebar.menu.settings, icon: Settings }
    ];

    return (
        <>
            {/* Sidebar */}
            <div className={`
                fixed left-0 top-0 bottom-0 w-64 bg-gray-900 z-50 transform transition-all duration-700 ease-in-out shadow-2xl
                ${isOpen ? 'translate-x-0 opacity-100' : '-translate-x-full lg:translate-x-0 lg:opacity-100 opacity-0'}
            `}>
                <div className="p-4 h-full overflow-y-auto">
                    {/* Mobile close button */}
                    <div className="flex items-center justify-between mb-6 lg:justify-start">
                        <div className={`flex items-start gap-2 transition-all duration-1000 ease-out transform ${isOpen ? 'translate-x-0 opacity-100' : '-translate-x-4 opacity-0 lg:translate-x-0 lg:opacity-100'}`}>
                            <Logo extraStye="" />
                            <span className="text-white font-bold text-[20px] mr-8.5 mt-1">{t(i18nKeyContainer.layout.sidebar.brand)}</span>
                        </div>
                        <button 
                            onClick={onClose}
                            className={`lg:hidden text-white hover:text-gray-300 p-1 transition-all duration-500 hover:rotate-90 transform ${isOpen ? 'scale-100 opacity-100' : 'scale-0 opacity-0'}`}
                        >
                            <X className="h-6 w-6" />
                        </button>
                    </div>
                    
                    <div className="flex gap-3 flex-col">
                        {menuItems.map((item, index) => (
                            <div 
                                key={item.pathname}
                                className={`transition-all duration-700 ease-out transform ${
                                    isOpen ? 'translate-x-0 opacity-100' : '-translate-x-8 opacity-0 lg:translate-x-0 lg:opacity-100'
                                }`}
                                style={{ 
                                    transitionDelay: isOpen ? `${(index + 1) * 150}ms` : `${(5 - index) * 100}ms`
                                }}
                            >
                                <SideBarLink 
                                    pathname={item.pathname} 
                                    content={t(item.contentKey)} 
                                    leftIcon={item.icon} 
                                />
                            </div>
                        ))}
                    </div>
                </div>
            </div>
        </>
    )
}