import { useMemo } from 'react';
import { useAuth } from '@/components/AuthProvider';
import { createApiClient } from '@/services/api';

export const useApi = () => {
  const { token } = useAuth();

  return useMemo(() => {
    if (!token) {
      return null;
    }

    return createApiClient(token);
  }, [token]);
};

