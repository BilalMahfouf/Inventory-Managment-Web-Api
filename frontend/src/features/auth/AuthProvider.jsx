import { useEffect, useState } from 'react';
import { tokenManager } from '@shared/services/api/api';
import { AuthContext } from './AuthContext';

export function AuthProvider({ children }) {
  const [isLoading, setIsLoading] = useState(true);
  const [isAuthenticated, setIsAuthenticated] = useState(() => {
    return Boolean(tokenManager.getAccessToken());
  });

  useEffect(() => {
    let isMounted = true;

    const unsubscribeToken = tokenManager.subscribe(token => {
      if (isMounted) {
        setIsAuthenticated(Boolean(token));
      }
    });

    const unsubscribeUnauthorized = tokenManager.onUnauthorized(() => {
      window.location.assign('/');
    });

    const initSession = async () => {
      if (tokenManager.getAccessToken()) {
        if (isMounted) {
          setIsLoading(false);
        }
        return;
      }

      try {
        await tokenManager.refreshAccessToken();
      } catch {
        // Refresh cookie missing/expired. Guard will redirect on protected pages.
      } finally {
        if (isMounted) {
          setIsLoading(false);
        }
      }
    };

    initSession();

    return () => {
      isMounted = false;
      unsubscribeToken();
      unsubscribeUnauthorized();
    };
  }, []);

  return (
    <AuthContext.Provider value={{ isAuthenticated, isLoading }}>
      {children}
    </AuthContext.Provider>
  );
}
