export default function Alert({ productName, description, time, status }) {
  return (
    <div
      className='bg-white rounded-xl   p-4
          hover:bg-gray-50 border border-gray-200 transition-colors '
    >
      <div className='flex items-center justify-between'>
        <div className='flex-1'>
          <h3 className='text-gray-900 font-medium text-sm mb-1'>
            {productName} - {description}
          </h3>
          <p className='text-gray-500 text-sm'>{time}</p>
        </div>
        <div className='ml-4'>
          <span
            className='inline-flex items-center px-3 py-1 rounded-full text-xs font-medium
           bg-orange-100 text-orange-800 border border-orange-200'
          >
            {status}
          </span>
        </div>
      </div>
    </div>
  );
}
