import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./features/dashboard/dashboard.component').then((m) => m.DashboardComponent),
    data: {
      title: 'FinSight AI — Dólar Blue, Oficial y Cripto en Argentina',
      description:
        'Cotización del dólar blue, oficial, MEP, CCL y cripto en Argentina en tiempo real. Conversor, histórico y análisis con IA.',
    },
  },
  {
    path: 'dashboard',
    redirectTo: '',
    pathMatch: 'full',
  },
  {
    path: 'portfolio',
    loadComponent: () =>
      import('./features/portfolio/portfolio.component').then((m) => m.PortfolioComponent),
    canActivate: [authGuard],
    data: {
      title: 'Mi Portfolio — FinSight AI',
      description:
        'Seguí el valor de tu portfolio de dólares y criptomonedas ajustado por inflación.',
    },
  },
  {
    path: 'analysis',
    loadComponent: () =>
      import('./features/analysis/analysis.component').then((m) => m.AnalysisComponent),
    canActivate: [authGuard],
    data: {
      title: 'Análisis con IA — FinSight AI',
      description: 'Análisis de tu portfolio y del contexto cambiario argentino generado con IA.',
    },
  },
  {
    path: 'auth/login',
    loadComponent: () =>
      import('./features/auth/login/login.component').then((m) => m.LoginComponent),
    data: {
      title: 'Ingresar — FinSight AI',
      description:
        'Ingresá a tu cuenta de FinSight AI para seguir tu portfolio y acceder al análisis con IA.',
    },
  },
  {
    path: 'auth/register',
    loadComponent: () =>
      import('./features/auth/register/register.component').then((m) => m.RegisterComponent),
    data: {
      title: 'Crear cuenta — FinSight AI',
      description:
        'Creá tu cuenta gratis en FinSight AI y empezá a seguir tu portfolio de dólares y cripto.',
    },
  },
  {
    path: '**',
    redirectTo: '',
  },
];
