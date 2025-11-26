import 'keycloak-js';

// src/types/keycloak.d.ts
declare module 'keycloak-js' {
    interface KeycloakProfile {
      realm_access?: {
        roles: string[];
      };
      resource_access?: Record<string, { roles: string[] }>;
    }
  }