import CartDescription from "./CartDescription";
import CartHeader from "./CartHeader";

export default function Cart() {
    
    return (
        <div className=" p-5
        w-[450px]  mx-auto
        
  bg-white/80 
  backdrop-blur-sm 
  border 
  border-gray-200 
  rounded-lg 
  shadow-lg 
  text-[#14161A] 
  font-sans 
  leading-6 
  block " >
            <CartHeader />
            <CartDescription />
        </div>
    )

}