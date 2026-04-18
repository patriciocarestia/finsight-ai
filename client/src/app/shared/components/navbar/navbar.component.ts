import { Component, inject, signal } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { Store } from '@ngrx/store';
import { AsyncPipe } from '@angular/common';
import { selectIsAuthenticated, selectEmail } from '../../../store/auth/auth.selectors';
import { logout } from '../../../store/auth/auth.actions';
import { ThemeService } from '../../../core/services/theme.service';

@Component({
  selector: 'app-navbar',
  imports: [RouterLink, RouterLinkActive, AsyncPipe],
  templateUrl: './navbar.component.html'
})
export class NavbarComponent {
  private readonly store = inject(Store);
  readonly theme = inject(ThemeService);
  readonly isAuthenticated$ = this.store.select(selectIsAuthenticated);
  readonly email$ = this.store.select(selectEmail);
  readonly showMenu = signal(false);

  onLogout() {
    this.showMenu.set(false);
    this.store.dispatch(logout());
  }
}
