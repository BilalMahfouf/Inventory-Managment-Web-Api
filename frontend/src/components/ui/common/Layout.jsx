import TopNav from "./TopNav";
import SideBar from "./Sidebar";

export default function Layout({ children }) {
    return (
        <div className="min-h-screen bg-gray-50">
            {/* Top Navigation - spans full width */}
            <TopNav />
            
            {/* Sidebar - fixed position */}
            <SideBar />
            
            {/* Main Content Area */}
            <main className="pt-16 lg:pl-64"> {/* pt-16 for topnav height, pl-64 for sidebar width on large screens */}
                <div className="p-6">
                    {children}
                </div>
            </main>
        </div>
    );
}