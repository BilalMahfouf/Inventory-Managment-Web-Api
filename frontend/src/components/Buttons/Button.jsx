import { Loader2 } from 'lucide-react';

const VARIANTS = {
  primary: 'bg-blue-600 text-white hover:bg-blue-700 active:bg-blue-800',
  secondary:
    'bg-white border border-gray-300 text-gray-700 hover:bg-gray-100 active:bg-gray-200',
  destructive: 'bg-red-600 text-white hover:bg-red-700 active:bg-red-800',
  disabled: 'bg-gray-300 text-gray-500 cursor-not-allowed',
};

export default function Button({
  children,
  variant = 'primary',
  disabled = false,
  loading = false,
  onClick = () => {},
  className = '',
  LeftIcon,
  ...props
}) {
  const isDisabled = disabled || loading;

  return (
    <button
      onClick={e => {
        if (!isDisabled) {
          onClick(e);
          console.log('button is clicked');
        }
      }}
      className={`
        relative inline-flex items-center justify-center
        px-4 py-2 rounded-lg font-semibold min-h-10 min-w-6
        transition-colors duration-200 cursor-pointer
        ${isDisabled ? VARIANTS.disabled : VARIANTS[variant]}
        ${className}
      `}
      disabled={isDisabled}
      {...props}
    >
      {/* Left Icon */}
      {LeftIcon && !loading && <LeftIcon className='h-5 w-5 mr-2' />}

      {/* Loading Spinner */}
      {loading && <Loader2 className='animate-spin mr-2 h-5 w-5' />}

      {/* Button Content */}
      {children}
    </button>
  );
}
