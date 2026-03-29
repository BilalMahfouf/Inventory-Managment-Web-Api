import { createContext, useContext } from 'react';

const AuthContext = createContext({
  isAuthenticated: false,
  isLoading: true,
});

function useAuthContext() {
  return useContext(AuthContext);
}

export { AuthContext, useAuthContext };
