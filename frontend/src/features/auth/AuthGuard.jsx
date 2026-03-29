import { Navigate, Outlet } from 'react-router-dom';
import { Loader2 } from 'lucide-react';
import { useAuthContext } from './AuthContext';
import { tokenManager } from '@shared/services/api/api';

export default function AuthGuard() {
  const { isAuthenticated, isLoading } = useAuthContext();
  const hasAccessToken = Boolean(tokenManager.getAccessToken());

  if (isLoading) {
    return (
      <div className="flex h-screen w-full items-center justify-center">
        <Loader2 className="h-8 w-8 animate-spin text-primary" />
      </div>
    );
  }

  if (!isAuthenticated && !hasAccessToken) {
    return <Navigate to="/" replace />;
  }

  return <Outlet />;
}
