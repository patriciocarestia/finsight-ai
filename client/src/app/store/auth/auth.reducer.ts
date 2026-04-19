import { createFeature, createReducer, on } from '@ngrx/store';
import { AuthState } from './auth.model';
import {
  login,
  loginFailure,
  loginSuccess,
  logout,
  register,
  registerFailure,
  registerSuccess,
} from './auth.actions';

const emptyState: AuthState = {
  token: null,
  email: null,
  expiresAt: null,
  loading: false,
  error: null,
};

function loadAuthFromStorage(): AuthState {
  try {
    const token = localStorage.getItem('auth_token');
    const email = localStorage.getItem('auth_email');
    const expiresAt = localStorage.getItem('auth_expires_at');
    if (token && email && expiresAt && new Date(expiresAt) > new Date()) {
      return { token, email, expiresAt, loading: false, error: null };
    }
  } catch (error) {
    console.warn('[Auth] Storage error:', error);
  }
  return emptyState;
}

const initialState: AuthState = loadAuthFromStorage();

export const authFeature = createFeature({
  name: 'auth',
  reducer: createReducer(
    initialState,

    on(login, (state) => ({
      ...state,
      loading: true,
      error: null,
    })),

    on(loginSuccess, (state, { token, email, expiresAt }) => ({
      ...state,
      token,
      email,
      expiresAt,
      loading: false,
      error: null,
    })),

    on(loginFailure, (state, { error }) => ({
      ...state,
      loading: false,
      error,
    })),

    on(register, (state) => ({
      ...state,
      loading: true,
      error: null,
    })),

    on(registerSuccess, (state, { token, email, expiresAt }) => ({
      ...state,
      token,
      email,
      expiresAt,
      loading: false,
      error: null,
    })),

    on(registerFailure, (state, { error }) => ({
      ...state,
      loading: false,
      error,
    })),

    on(logout, () => emptyState),
  ),
});
