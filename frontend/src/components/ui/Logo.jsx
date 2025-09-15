import { Lock } from "lucide-react"



export default function Logo(extraStyle="mx-auto") {
    const style=`bg-primary-blue w-12 h-12  rounded-xl pt-2 pl-2 mb-3 ${extraStyle}`;
    return (
        <div className={style}>
               <Lock size={30} color="#ffffff" absoluteStrokeWidth  c/>
            </div>
    )
}