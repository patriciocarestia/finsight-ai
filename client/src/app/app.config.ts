import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideStore } from '@ngrx/store';
import { provideEffects } from '@ngrx/effects';
import { provideStoreDevtools } from '@ngrx/store-devtools';
import { isDevMode } from '@angular/core';

import { routes } from './app.routes';
import { authInterceptor } from './core/interceptors/auth.interceptor';
import { authFeature } from './store/auth/auth.reducer';
import { ratesFeature } from './store/rates/rates.reducer';
import { portfolioFeature } from './store/portfolio/portfolio.reducer';
import { analysisFeature } from './store/analysis/analysis.reducer';
import { AuthEffects } from './store/auth/auth.effects';
import { RatesEffects } from './store/rates/rates.effects';
import { PortfolioEffects } from './store/portfolio/portfolio.effects';
import { AnalysisEffects } from './store/analysis/analysis.effects';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(withInterceptors([authInterceptor])),
    provideStore({
      [authFeature.name]: authFeature.reducer,
      [ratesFeature.name]: ratesFeature.reducer,
      [portfolioFeature.name]: portfolioFeature.reducer,
      [analysisFeature.name]: analysisFeature.reducer
    }),
    provideEffects([AuthEffects, RatesEffects, PortfolioEffects, AnalysisEffects]),
    provideStoreDevtools({ maxAge: 25, logOnly: !isDevMode() })
  ]
};
