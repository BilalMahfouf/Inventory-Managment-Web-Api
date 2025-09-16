






export default function InfoCard({
    title,number,description,status,statusLabel,iconComponent:IconComponent,className
}) {

// status = true means increase in number, false means decrease in number
const statusLabelClass=
 status  ? 'bg-green-100 text-green-700' : 'bg-red-100 text-red-700';

    return (
        <div className={`bg-white rounded-lg shadow-md border-gray-200 p-6 min-w-36 transition-all duration-300
         ease-in-out hover:shadow-lg hover:-translate-y-1 ${className}`}>
        <div className="flex justify-between items-center mb-2">
        <h2 className="text-gray-600 text-sm font-medium">{title}</h2>
        {IconComponent && (
          <div className=" p-2 rounded-md">
            <IconComponent className="w-5 h-5 text-gray-400" />
          </div>
        )}
    </div>

    <div className="flex items-center justify-between">
        <div className="flex flex-col">
            <p className="text-2xl font-bold text-gray-900">{number}</p>
            <p className="text-gray-500 text-xs mt-1">{description}</p>
        </div>
        <div className={` ${statusLabelClass} px-3 py-1 rounded-full text-xs font-semibold`}>{statusLabel}</div>
    </div>
</div>
    )


}