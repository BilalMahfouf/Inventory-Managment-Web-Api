import { RouterProvider } from 'react-router-dom';
import { QueryClientProvider } from '@tanstack/react-query';
import { AuthProvider } from '@features/auth/AuthProvider';
import { ToastProvider } from '@shared/context/ToastContext';
import queryClient from '@shared/lib/queryClient';
import { router } from './router';

function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <ToastProvider>
        <AuthProvider>
          <RouterProvider router={router} />
        </AuthProvider>
      </ToastProvider>
    </QueryClientProvider>
  );
}

export default App;
