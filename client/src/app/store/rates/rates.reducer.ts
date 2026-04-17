import { createFeature, createReducer, on } from '@ngrx/store';
import {
  loadHistory,
  loadHistoryFailure,
  loadHistorySuccess,
  loadRates,
  loadRatesFailure,
  loadRatesSuccess,
} from './rates.actions';
import { CryptoRate, ExchangeRate } from './rates.model';

export interface RatesState {
  exchangeRates: ExchangeRate[];
  cryptoRates: CryptoRate[];
  history: ExchangeRate[];
  loading: boolean;
  error: string | null;
  lastFetched: string | null;
}

const initialState: RatesState = {
  exchangeRates: [],
  cryptoRates: [],
  history: [],
  loading: false,
  error: null,
  lastFetched: null,
};

export const ratesFeature = createFeature({
  name: 'rates',
  reducer: createReducer(
    initialState,

    on(loadRates, (state) => ({
      ...state,
      loading: true,
      error: null,
    })),

    on(loadRatesSuccess, (state, { exchangeRates, cryptoRates }) => ({
      ...state,
      exchangeRates,
      cryptoRates,
      loading: false,
      error: null,
      lastFetched: new Date().toISOString(),
    })),

    on(loadRatesFailure, (state, { error }) => ({
      ...state,
      loading: false,
      error,
    })),

    on(loadHistory, (state) => ({
      ...state,
      loading: true,
      error: null,
    })),

    on(loadHistorySuccess, (state, { history }) => ({
      ...state,
      history,
      loading: false,
      error: null,
    })),

    on(loadHistoryFailure, (state, { error }) => ({
      ...state,
      loading: false,
      error,
    }))
  ),
});
