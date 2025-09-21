import { useState } from "react";
import TopNav from "./TopNav";
import Sidebar from "./Sidebar";

export default function Layout({ children }) {
    const [sidebarOpen, setSidebarOpen] = useState(false);

    const toggleSidebar = () => {
        setSidebarOpen(!sidebarOpen);
    };

    const closeSidebar = () => {
        setSidebarOpen(false);
    };

    return (
        <div className="min-h-screen bg-gray-50">
            {/* Sidebar - responsive with state management */}
            <Sidebar isOpen={sidebarOpen} onClose={closeSidebar} />
            
            {/* Top Navigation - responsive */}
            <TopNav onToggleSidebar={toggleSidebar} />
            
            {/* Main Content Area - responsive padding */}
            <main className="pt-16 lg:pl-64 transition-all duration-300">
                <div className="p-4 md:p-6">
                    {children}
                </div>
            </main>
        </div>
    );
}