'use client';

import React, { createContext, useContext, useEffect, useMemo, useState } from 'react';
import Keycloak from 'keycloak-js';

const keycloak = new Keycloak({
  url: process.env.NEXT_PUBLIC_KEYCLOAK_URL || 'http://localhost:8080',
  realm: process.env.NEXT_PUBLIC_KEYCLOAK_REALM || 'hospital',
  clientId: process.env.NEXT_PUBLIC_KEYCLOAK_CLIENT_ID || 'hospital-frontend',
});

interface AuthContextType {
  isAuthenticated: boolean;
  token?: string;
  userProfile: Keycloak.KeycloakProfile | null;
  logout: () => void;
}

const AuthContext = createContext<AuthContextType>({
  isAuthenticated: false,
  token: undefined,
  userProfile: null,
  logout: () => {},
});

export const AuthProvider = ({ children }: { children: React.ReactNode }) => {
  const [isInitialized, setIsInitialized] = useState(false);
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [token, setToken] = useState<string | undefined>(undefined);
  const [userProfile, setUserProfile] = useState<Keycloak.KeycloakProfile | null>(null);

  useEffect(() => {
    if (typeof window === 'undefined') {
      return;
    }

    const init = async () => {
      try {
        const authenticated = await keycloak.init({
          onLoad: 'login-required',
          checkLoginIframe: false,
          pkceMethod: 'S256',
        });

        setIsAuthenticated(authenticated);
        setToken(keycloak.token);

        if (authenticated) {
          const profile = await keycloak.loadUserProfile();
          setUserProfile(profile);
        }

        setIsInitialized(true);
      } catch (error) {
        console.error('Keycloak init failed', error);
        setIsInitialized(true);
      }
    };

    init();

    const refreshInterval = setInterval(async () => {
      try {
        const refreshed = await keycloak.updateToken(60);
        if (refreshed) {
          setToken(keycloak.token);
        }
      } catch (error) {
        console.error('Failed to refresh token', error);
      }
    }, 30_000);

    return () => {
      clearInterval(refreshInterval);
    };
  }, []);

  const logout = () => {
    keycloak.logout();
  };

  const value = useMemo(
    () => ({
      isAuthenticated,
      token,
      userProfile,
      logout,
    }),
    [isAuthenticated, token, userProfile],
  );

  if (!isInitialized) {
    return (
      <div className="flex h-screen items-center justify-center text-black">
        Loading authentication...
      </div>
    );
  }

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};

export const useAuth = () => useContext(AuthContext);
