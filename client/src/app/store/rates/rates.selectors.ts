import { createSelector } from '@ngrx/store';
import { ratesFeature } from './rates.reducer';

export const selectRatesState = ratesFeature.selectRatesState;

export const selectExchangeRates = createSelector(
  selectRatesState,
  (state) => state.exchangeRates
);

export const selectCryptoRates = createSelector(
  selectRatesState,
  (state) => state.cryptoRates
);

export const selectRatesHistory = createSelector(
  selectRatesState,
  (state) => state.history
);

export const selectRatesLoading = createSelector(
  selectRatesState,
  (state) => state.loading
);

export const selectRatesError = createSelector(
  selectRatesState,
  (state) => state.error
);

export const selectLastFetched = createSelector(
  selectRatesState,
  (state) => state.lastFetched
);
