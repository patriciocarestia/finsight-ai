import { createFeature, createReducer, on } from '@ngrx/store';
import {
  analyzePortfolio,
  analyzePortfolioFailure,
  analyzePortfolioSuccess,
} from './analysis.actions';

export interface AnalysisState {
  analysis: string | null;
  generatedAt: string | null;
  loading: boolean;
  error: string | null;
}

const initialState: AnalysisState = {
  analysis: null,
  generatedAt: null,
  loading: false,
  error: null,
};

export const analysisFeature = createFeature({
  name: 'analysis',
  reducer: createReducer(
    initialState,

    on(analyzePortfolio, (state) => ({
      ...state,
      loading: true,
      error: null,
    })),

    on(analyzePortfolioSuccess, (state, { analysis, generatedAt }) => ({
      ...state,
      analysis,
      generatedAt,
      loading: false,
      error: null,
    })),

    on(analyzePortfolioFailure, (state, { error }) => ({
      ...state,
      loading: false,
      error,
    })),
  ),
});
