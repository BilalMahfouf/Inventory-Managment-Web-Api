





export default function PageHeader({ title, description, className="",containerClass=""}) {


    return (
        <div className={containerClass}>
                <h1 className={`text-3xl font-bold text-gray-900 mb-1 ${className}`}>{title}</h1>
                <p className={`text-gray-600  ${className}`}>{description}</p>
                </div>
    )

}