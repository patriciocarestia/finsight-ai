import { RenderMode, ServerRoute } from '@angular/ssr';

export const serverRoutes: ServerRoute[] = [
  {
    path: '',
    renderMode: RenderMode.Prerender,
  },
  {
    path: 'dashboard',
    renderMode: RenderMode.Prerender,
  },
  {
    path: 'auth/login',
    renderMode: RenderMode.Prerender,
  },
  {
    path: 'auth/register',
    renderMode: RenderMode.Prerender,
  },
  {
    // Portfolio and analysis are behind an auth guard that reads
    // localStorage, which doesn't exist during prerendering — render
    // them client-side only.
    path: '**',
    renderMode: RenderMode.Client,
  },
];
