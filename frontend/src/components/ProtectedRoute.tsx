import { ReactNode } from 'react';
import { useAuth } from './AuthProvider';

interface ProtectedRouteProps {
  roles?: string[];
  fallback?: ReactNode;
  children: ReactNode;
}

export default function ProtectedRoute({ roles, fallback, children }: ProtectedRouteProps) {
  const { isAuthenticated, userProfile } = useAuth();

  if (!isAuthenticated) {
    return fallback ?? <p className="text-gray-600">Authenticating...</p>;
  }

  if (roles && roles.length > 0) {
    const userRoles = userProfile?.realmAccess?.roles ?? [];
    const hasRole = roles.some((role) => userRoles.includes(role));

    if (!hasRole) {
      return fallback ?? <p className="text-red-600">You do not have access to view this section.</p>;
    }
  }

  return <>{children}</>;
}

