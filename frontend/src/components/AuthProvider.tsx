'use client';

import React, { createContext, useContext, useEffect, useMemo, useState } from 'react';
import Keycloak from 'keycloak-js';

// Correct Keycloak field name: realm_access (underscore!)
export interface ExtendedKeycloakProfile extends Keycloak.KeycloakProfile {
  realm_access?: {
    roles: string[];
  };
}

const keycloak = new Keycloak({
  url: process.env.NEXT_PUBLIC_KEYCLOAK_URL || 'http://localhost:8080',
  realm: process.env.NEXT_PUBLIC_KEYCLOAK_REALM || 'hospital',
  clientId: process.env.NEXT_PUBLIC_KEYCLOAK_CLIENT_ID || 'hospital-frontend',
});

interface AuthContextType {
  isAuthenticated: boolean;
  token?: string;
  userProfile: ExtendedKeycloakProfile | null;
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
  const [token, setToken] = useState<string | undefined>();
  const [userProfile, setUserProfile] = useState<ExtendedKeycloakProfile | null>(null);

  useEffect(() => {
    if (typeof window === 'undefined') return;

    keycloak
      .init({
        onLoad: 'login-required',
        checkLoginIframe: false,
        pkceMethod: 'S256',
      })
      .then((authenticated) => {
        setIsAuthenticated(authenticated);
        setToken(keycloak.token);

        if (authenticated) {
          return keycloak.loadUserProfile();
        }
      })
      .then((profile) => {
        if (profile) {
          setUserProfile(profile as ExtendedKeycloakProfile);
        }
      })
      .catch((error) => {
        console.error('Keycloak init failed', error);
      })
      .finally(() => {
        setIsInitialized(true);
      });

    const interval = setInterval(() => {
      keycloak.updateToken(60).catch(() => {
        console.error('Failed to refresh token');
      });
    }, 30_000);

    return () => clearInterval(interval);
  }, []);

  const logout = () => keycloak.logout();

  const value = useMemo(
    () => ({ isAuthenticated, token, userProfile, logout }),
    [isAuthenticated, token, userProfile]
  );

  if (!isInitialized) {
    return (
      <div className="flex h-screen items-center justify-center">
        Loading authentication...
      </div>
    );
  }

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};

export const useAuth = () => useContext(AuthContext);