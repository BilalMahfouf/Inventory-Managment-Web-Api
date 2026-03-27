import { Package } from "lucide-react"



export default function Logo({extraStyle="mx-auto",size=20}) {
    const style=`bg-primary-blue ${size===36?"w-12 h-12" : "w-9 h-9"} ${extraStyle} rounded-xl pt-2 pl-2 mb-3 `;
    return (
        <div className={style}>
               <Package size={size} color="#ffffff" absoluteStrokeWidth  />
            </div>
    )
}