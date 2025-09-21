import Action from "./Action"
import { Package, Users2, ShoppingCart, MapPin } from "lucide-react"





export default function QuickActions( {className = "" } ) {



    return (
        <div className={`grid grid-cols-2 gap-2 ${className}`}>
            <Action 
                title="Add Product" 
                icon={<Package />}
            />
            <Action 
            title="Add Customer"
            icon={<Users2 />}
            theme="purple"
            />
            <Action 
            title="New Sale Order"
            icon={<ShoppingCart />}
            theme="green"
            />
            <Action 
            title="Add Location"
            icon={<MapPin />}
            theme="orange"
            />
        </div>
    )


}