import { createSelector } from '@ngrx/store';
import { authFeature } from './auth.reducer';

export const selectAuthState = authFeature.selectAuthState;

export const selectToken = createSelector(selectAuthState, (state) => state.token);

export const selectEmail = createSelector(selectAuthState, (state) => state.email);

export const selectIsAuthenticated = createSelector(
  selectAuthState,
  (state) => state.token !== null,
);

export const selectAuthLoading = createSelector(selectAuthState, (state) => state.loading);

export const selectAuthError = createSelector(selectAuthState, (state) => state.error);
