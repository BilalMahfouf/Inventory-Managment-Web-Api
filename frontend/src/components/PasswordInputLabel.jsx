import { useState } from "react";


export default  function PasswordInputLabel({
  placeholder,
  leftIcon:LeftIcon,
  rightIcon:RightIcon,
  rightIconReplacment:RightIconReplacment,
  error,
  label,
  onChange,
  ...props
}) {
    const [isHidden,setIsHidden] = useState(true);
   
    const showHidePasswordClick=()=>{
        setIsHidden(!isHidden);
    }


  return (
    <div className="max-w-[400px] w-full grid justify-items-start">

    <label className="block text-sm font-medium text-gray-900 mb-1.5">
        {label}
      </label>
      
      <div className="relative w-full   gap-3">
        {/* Left Icon */}
        {LeftIcon && (
          <div className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 pointer-events-none">
            <LeftIcon className="h-5 w-5 mr-3" />
          </div>
        )}

      <input
        type={isHidden?"password":"text"} onChange={onChange}
        placeholder={placeholder}
        className={`
          w-full h-11 px-4 rounded-lg text-base bg-gray-50
          border ${error ? "border-red-500 focus:ring-red-500 focus:border-red-500" : "border-gray-300 focus:ring-blue-500 focus:border-blue-500"}
          focus:outline-none focus:ring-2
          transition-colors  ${LeftIcon ? "pl-10" : ""}  
        `}
        {...props}
      />
      
        {/* Right Icon */}
        {RightIcon && (
          <button type="button" className="absolute right-3 top-1/2 transform -translate-y-1/2 text-gray-400 
          cursor-pointer z-10 hover:text-gray-900" onClick={()=>showHidePasswordClick()} >
            {isHidden?<RightIcon className="h-5 w-5"/>:<RightIconReplacment className="h-5 w-5" />}
            
          </button>
        )}
      </div>

      {error && (
        <p className="text-sm text-red-600 mt-1">{error}</p>
      )}

       
    </div>
  )
}
