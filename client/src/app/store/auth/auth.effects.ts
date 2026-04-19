import { Injectable, inject } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { catchError, map, switchMap, tap } from 'rxjs/operators';
import { AuthService } from '../../core/services/auth.service';
import * as AuthActions from './auth.actions';

@Injectable()
export class AuthEffects {
  private readonly actions$ = inject(Actions);
  private readonly authService = inject(AuthService);

  login$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AuthActions.login),
      switchMap(({ email, password }) =>
        this.authService.login({ email, password }).pipe(
          map((response) =>
            AuthActions.loginSuccess({
              token: response.token,
              email: response.email,
              expiresAt: response.expiresAt,
            }),
          ),
          catchError((err) =>
            of(
              AuthActions.loginFailure({
                error: err.error?.error ?? 'Login failed. Please try again.',
              }),
            ),
          ),
        ),
      ),
    ),
  );

  register$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AuthActions.register),
      switchMap(({ email, password }) =>
        this.authService.register({ email, password }).pipe(
          map((response) =>
            AuthActions.registerSuccess({
              token: response.token,
              email: response.email,
              expiresAt: response.expiresAt,
            }),
          ),
          catchError((err) =>
            of(
              AuthActions.registerFailure({
                error: err.error?.error ?? 'Registration failed. Please try again.',
              }),
            ),
          ),
        ),
      ),
    ),
  );

  persistSession$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(AuthActions.loginSuccess, AuthActions.registerSuccess),
        tap(({ token, email, expiresAt }) => {
          localStorage.setItem('auth_token', token);
          localStorage.setItem('auth_email', email);
          localStorage.setItem('auth_expires_at', expiresAt);
        }),
      ),
    { dispatch: false },
  );

  clearSession$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(AuthActions.logout),
        tap(() => {
          localStorage.removeItem('auth_token');
          localStorage.removeItem('auth_email');
          localStorage.removeItem('auth_expires_at');
        }),
      ),
    { dispatch: false },
  );
}
