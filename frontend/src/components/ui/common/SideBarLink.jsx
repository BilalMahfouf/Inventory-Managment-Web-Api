import { NavLink } from "react-router-dom";

export default function SideBarLink({
  content,
  pathname,
  search = null,
  hash = null,
  leftIcon: LeftIcon,
}) {
  return (
    <NavLink
      to={{
        pathname,
        search: search || undefined,
        hash: hash || undefined,
      }}
      className={({ isActive }) =>
        `group flex items-center py-2 text-sm font-medium rounded-lg transition-colors border-l-4 ${
          isActive
            ?  "bg-blue-600 text-white border-blue-400 px-4"
            : "text-gray-300 border-transparent hover:bg-gray-800 hover:text-white px-3"
        }`
      }
    >
      {({ isActive }) => (
        <>
          {LeftIcon && (
            <LeftIcon
              className={`h-5 w-5 mr-3 ${
                isActive ? "text-white" : "text-gray-400 group-hover:text-white"
              }`}
            />
          )}
          {content}
        </>
      )}
    </NavLink>
  );
}
