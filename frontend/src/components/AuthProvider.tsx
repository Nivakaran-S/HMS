'use client';

import React, { createContext, useContext, useEffect, useState } from 'react';
import Keycloak from 'keycloak-js';

const client = new Keycloak({
  url: process.env.NEXT_PUBLIC_KEYCLOAK_URL || 'http://localhost:8080',
  realm: process.env.NEXT_PUBLIC_KEYCLOAK_REALM || 'hospital',
  clientId: process.env.NEXT_PUBLIC_KEYCLOAK_CLIENT_ID || 'hospital-frontend',
});

interface AuthContextType {
  isAuthenticated: boolean;
  token: string | undefined;
  logout: () => void;
  userProfile: any;
}

const AuthContext = createContext<AuthContextType>({
  isAuthenticated: false,
  token: undefined,
  logout: () => {},
  userProfile: null,
});

export const AuthProvider = ({ children }: { children: React.ReactNode }) => {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [token, setToken] = useState<string | undefined>(undefined);
  const [userProfile, setUserProfile] = useState<any>(null);
  const [isInitialized, setIsInitialized] = useState(false);

  useEffect(() => {
    // Prevent running on server side
    if (typeof window === 'undefined') return;

    const initKeycloak = async () => {
      try {
        const authenticated = await client.init({
          onLoad: 'login-required',
          checkLoginIframe: false,
        });
        setIsAuthenticated(authenticated);
        setToken(client.token);
        
        if (authenticated) {
          const profile = await client.loadUserProfile();
          setUserProfile(profile);
        }
        setIsInitialized(true);
      } catch (error) {
        console.error("Keycloak init failed", error);
        setIsInitialized(true); // Stop loading state even if failed
      }
    };

    initKeycloak();
  }, []);

  const logout = () => {
    client.logout();
  };

  if (!isInitialized) {
    return <div className="flex h-screen items-center justify-center text-black">Loading Authentication...</div>;
  }

  return (
    <AuthContext.Provider value={{ isAuthenticated, token, logout, userProfile }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => useContext(AuthContext);