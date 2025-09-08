import { Loader2 } from "lucide-react";

const VARIANTS = {
  primary: "bg-blue-600 text-white hover:bg-blue-700 active:bg-blue-800",
  secondary:
    "bg-white border border-gray-300 text-gray-700 hover:bg-gray-100 active:bg-gray-200",
  destructive:
    "bg-red-600 text-white hover:bg-red-700 active:bg-red-800",
  disabled: "bg-gray-300 text-gray-500 cursor-not-allowed",
};

export default function Button({
  children,
  variant = "primary",
  disabled = false,
  loading = false,
  ...props
}) {
  const isDisabled = disabled || loading;

  return (
    <button
      className={` 
        inline-flex items-center justify-center
        px-6 py-3 rounded-lg font-semibold h-11 w-[400px]
        transition-colors duration-200 
        ${isDisabled ? VARIANTS.disabled : VARIANTS[variant]}
      `}
      disabled={isDisabled}
      {...props}
    >
      {loading && <Loader2 className="animate-spin mr-2 h-5 w-5" />}
      {children}
    </button>
  );
}
