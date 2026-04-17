import { createAction, props } from '@ngrx/store';
import { CryptoRate, ExchangeRate } from './rates.model';

export const loadRates = createAction('[Rates] Load Rates');

export const loadRatesSuccess = createAction(
  '[Rates] Load Rates Success',
  props<{ exchangeRates: ExchangeRate[]; cryptoRates: CryptoRate[] }>()
);

export const loadRatesFailure = createAction(
  '[Rates] Load Rates Failure',
  props<{ error: string }>()
);

export const loadHistory = createAction(
  '[Rates] Load History',
  props<{ rateType: string; days: number }>()
);

export const loadHistorySuccess = createAction(
  '[Rates] Load History Success',
  props<{ history: ExchangeRate[] }>()
);

export const loadHistoryFailure = createAction(
  '[Rates] Load History Failure',
  props<{ error: string }>()
);
