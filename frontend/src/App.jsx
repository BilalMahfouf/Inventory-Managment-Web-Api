import { RouterProvider } from 'react-router-dom';
import { AuthProvider } from '@features/auth/AuthProvider';
import { ToastProvider } from '@shared/context/ToastContext';
import { router } from './router';

function App() {
  return (
    <ToastProvider>
      <AuthProvider>
        <RouterProvider router={router} />
      </AuthProvider>
    </ToastProvider>
  );
}

export default App;
