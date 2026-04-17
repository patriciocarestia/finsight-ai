import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { Store } from '@ngrx/store';
import { AsyncPipe } from '@angular/common';
import { login } from '../../../store/auth/auth.actions';
import { selectAuthLoading, selectAuthError, selectIsAuthenticated } from '../../../store/auth/auth.selectors';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule, RouterLink, AsyncPipe],
  template: `
    <div class="min-h-[80vh] flex items-center justify-center">
      <div class="card w-full max-w-md">
        <h1 class="text-2xl font-bold text-white mb-2">Iniciar sesión</h1>
        <p class="text-slate-400 mb-6 text-sm">Ingresá a tu cuenta de FinSight AI</p>

        @if (error$ | async; as error) {
          <div class="bg-red-900/30 border border-red-700 text-red-400 rounded-lg px-4 py-3 mb-4 text-sm">
            {{ error }}
          </div>
        }

        <form [formGroup]="form" (ngSubmit)="onSubmit()" class="space-y-4">
          <div>
            <label class="label">Email</label>
            <input type="email" formControlName="email" class="input" placeholder="tu@email.com" />
          </div>
          <div>
            <label class="label">Contraseña</label>
            <input type="password" formControlName="password" class="input" placeholder="••••••" />
          </div>
          <button type="submit"
                  class="btn-primary w-full"
                  [disabled]="form.invalid || (loading$ | async)">
            @if (loading$ | async) { Ingresando... } @else { Ingresar }
          </button>
        </form>

        <p class="text-center text-slate-400 text-sm mt-6">
          ¿No tenés cuenta?
          <a routerLink="/auth/register" class="text-blue-400 hover:text-blue-300">Registrate</a>
        </p>
      </div>
    </div>
  `
})
export class LoginComponent {
  private readonly store = inject(Store);
  private readonly fb = inject(FormBuilder);
  private readonly router = inject(Router);

  readonly loading$ = this.store.select(selectAuthLoading);
  readonly error$ = this.store.select(selectAuthError);

  readonly form = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]]
  });

  constructor() {
    this.store.select(selectIsAuthenticated).pipe(
      filter(Boolean),
      takeUntilDestroyed()
    ).subscribe(() => this.router.navigate(['/portfolio']));
  }

  onSubmit() {
    if (this.form.valid) {
      const { email, password } = this.form.getRawValue();
      this.store.dispatch(login({ email, password }));
    }
  }
}
