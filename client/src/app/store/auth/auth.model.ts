export interface AuthState {
  token: string | null;
  email: string | null;
  expiresAt: string | null;
  loading: boolean;
  error: string | null;
}
