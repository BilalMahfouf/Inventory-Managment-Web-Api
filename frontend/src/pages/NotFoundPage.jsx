
// this page is generated using ai 


import React from 'react';
import { AlertTriangle, ArrowLeft, Home } from 'lucide-react';

const NotFoundPage = () => {
  const handleGoBack = () => {
    window.history.back();
  };

  const handleReturnHome = () => {
    // In a real app, you'd use your router's navigation method
    // For example, with React Router: navigate('/') or history.push('/')
    window.location.href = '/';
  };

  return (
    <div className="min-h-screen bg-gray-50 flex items-center justify-center px-4">
      <div className="max-w-md w-full text-center">
        {/* Warning Icon */}
        <div className="mb-8">
          <div className="inline-flex items-center justify-center w-16 h-16 bg-red-100 rounded-full">
            <AlertTriangle className="w-8 h-8 text-red-500" />
          </div>
        </div>

        {/* 404 Heading */}
        <h1 className="text-6xl font-bold text-gray-900 mb-4">404</h1>
        
        {/* Page Not Found */}
        <h2 className="text-2xl font-semibold text-gray-900 mb-4">Page Not Found</h2>
        
        {/* Description */}
        <p className="text-gray-600 mb-8 leading-relaxed">
          The page you're looking for doesn't exist or has been moved.
        </p>

        {/* Action Buttons */}
        <div className="flex flex-col sm:flex-row gap-3 justify-center">
          <button
            onClick={handleGoBack}
            className="cursor-pointer  inline-flex items-center justify-center px-6 py-3 border border-gray-300 rounded-lg text-gray-700 bg-white hover:bg-gray-50 transition-colors duration-200 font-medium"
          >
            <ArrowLeft className="w-4 h-4 mr-2" />
            Go Back
          </button>
          
          <button
            onClick={handleReturnHome}
            className="cursor-pointer  inline-flex items-center justify-center px-6 py-3 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors duration-200 font-medium"
          >
            <Home className="w-4 h-4 mr-2" />
            Return Home
          </button>
        </div>

        {/* Optional: Additional help text */}
        <div className="mt-12 text-sm text-gray-500">
          <p>If you believe this is an error, please contact support.</p>
        </div>
      </div>
    </div>
  );
};

export default NotFoundPage;