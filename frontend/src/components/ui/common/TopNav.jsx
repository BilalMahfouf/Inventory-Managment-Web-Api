import { Search, Bell } from "lucide-react";

export default function TopNav() {
    return (
        <nav className="fixed top-0 left-0 right-0 z-50 bg-white border-b border-gray-200 h-16">
            <div className="flex items-center justify-between h-full px-6 lg:pl-72"> {/* pl-72 to account for sidebar width on large screens */}
                {/* Search Bar */}
                <div className="flex-1 max-w-lg">
                    <div className="relative">
                        <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                            <Search className="h-5 w-5 text-gray-400" />
                        </div>
                        <input
                            type="text"
                            placeholder="Search products, orders, customers..."
                            className="block w-full pl-10 pr-3 py-2.5 border border-gray-200 rounded-lg leading-5 bg-gray-50 placeholder-gray-500 focus:outline-none focus:placeholder-gray-400 focus:ring-1 focus:ring-blue-500 focus:border-blue-500 focus:bg-white text-sm"
                        />
                    </div>
                </div>

                {/* Right Side - Notifications and User */}
                <div className="flex items-center space-x-6">
                    {/* Notifications */}
                    <button className="relative p-1 text-gray-500 hover:text-gray-700 focus:outline-none">
                        <Bell className="h-6 w-6" />
                        {/* Notification badge */}
                        <span className="absolute -top-1 -right-1 flex items-center justify-center h-5 w-5 text-xs font-bold text-white bg-red-500 rounded-full">
                            3
                        </span>
                    </button>

                    {/* User Profile */}
                    <div className="flex items-center space-x-2">
                        <div className="flex items-center justify-center h-8 w-8 rounded-full bg-gray-700 text-white font-semibold text-sm">
                            JD
                        </div>
                    </div>
                </div>
            </div>
        </nav>
    );
}