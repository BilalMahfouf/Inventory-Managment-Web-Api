import { Package } from "lucide-react"



export default function Logo(extraStyle="mx-auto") {
    const style=`bg-primary-blue w-9 h-9  rounded-xl pt-2 pl-2 mb-3 ${extraStyle}`;
    return (
        <div className={style}>
               <Package size={20} color="#ffffff" absoluteStrokeWidth  c/>
            </div>
    )
}