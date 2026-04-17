import { Component, inject } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { Store } from '@ngrx/store';
import { AsyncPipe } from '@angular/common';
import { selectIsAuthenticated, selectEmail } from '../../../store/auth/auth.selectors';
import { logout } from '../../../store/auth/auth.actions';

@Component({
  selector: 'app-navbar',
  imports: [RouterLink, RouterLinkActive, AsyncPipe],
  template: `
    <nav class="bg-slate-900 border-b border-slate-800 sticky top-0 z-50">
      <div class="max-w-7xl mx-auto px-4 flex items-center justify-between h-16">
        <a routerLink="/dashboard" class="flex items-center gap-2 text-xl font-bold text-white">
          <span class="text-blue-500">&#9650;</span>
          FinSight AI
        </a>

        <div class="flex items-center gap-6">
          <a routerLink="/dashboard"
             routerLinkActive="text-blue-400"
             class="text-slate-400 hover:text-white transition-colors text-sm font-medium">
            Dashboard
          </a>
          @if (isAuthenticated$ | async) {
            <a routerLink="/portfolio"
               routerLinkActive="text-blue-400"
               class="text-slate-400 hover:text-white transition-colors text-sm font-medium">
              Portfolio
            </a>
            <a routerLink="/analysis"
               routerLinkActive="text-blue-400"
               class="text-slate-400 hover:text-white transition-colors text-sm font-medium">
              Análisis IA
            </a>
            <span class="text-slate-500 text-sm">{{ email$ | async }}</span>
            <button (click)="onLogout()"
                    class="text-slate-400 hover:text-red-400 transition-colors text-sm font-medium">
              Salir
            </button>
          } @else {
            <a routerLink="/auth/login"
               class="text-slate-400 hover:text-white transition-colors text-sm font-medium">
              Ingresar
            </a>
            <a routerLink="/auth/register"
               class="btn-primary text-sm">
              Registrarse
            </a>
          }
        </div>
      </div>
    </nav>
  `
})
export class NavbarComponent {
  private readonly store = inject(Store);
  readonly isAuthenticated$ = this.store.select(selectIsAuthenticated);
  readonly email$ = this.store.select(selectEmail);

  onLogout() {
    this.store.dispatch(logout());
  }
}
