import React from 'react';

// Theme configurations
const themes = {
  blue: {
    bgColor: 'bg-blue-100',
    textColor: 'text-blue-600',
    hoverColor: 'hover:bg-blue-200'
  },
  green: {
    bgColor: 'bg-green-100',
    textColor: 'text-green-600',
    hoverColor: 'hover:bg-green-200'
  },
  purple: {
    bgColor: 'bg-purple-100',
    textColor: 'text-purple-600',
    hoverColor: 'hover:bg-purple-200'
  },
  orange: {
    bgColor: 'bg-orange-100',
    textColor: 'text-orange-600',
    hoverColor: 'hover:bg-orange-200'
  }
};

const Action = ({ 
  title, 
  icon, 
  theme = 'blue', 
  onClick, 
  className = '' 
}) => {
  const selectedTheme = themes[theme] || themes.blue;

  return (
    <button
      type="button"
      className={`
        ${selectedTheme.bgColor} 
        ${selectedTheme.textColor} 
        ${selectedTheme.hoverColor}
        rounded-lg p-4 transition-all duration-200 
        transform hover:scale-105 hover:shadow-lg
        flex flex-col items-center justify-center text-center
         w-full aspect-square lg:max-w-43 lg:max-h-25 max-w-[417px] max-h-16
         focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500
         active:scale-95 cursor-pointer
        ${className}
      `}
      onClick={onClick}
    >
      <div className="mb-2">
        {React.cloneElement(icon, { className: 'w-5 h-5' })}
      </div>
      <h3 className="text-sm font-semibold leading-tight">
        {title}
      </h3>
    </button>
  );
};

export default Action;
