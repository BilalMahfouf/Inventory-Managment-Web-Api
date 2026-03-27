


export default function Link({to,content}) {
// blue-600 is the main blue color
    return (
        <div className="inline">
            <a href={to} className="font-medium text-blue-600 dark:text-blue-500 hover:underline">
                {content}
            </a>
        </div>
    )

}