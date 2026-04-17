import { createSelector } from '@ngrx/store';
import { analysisFeature } from './analysis.reducer';

export const selectAnalysisState = analysisFeature.selectAnalysisState;

export const selectAnalysis = createSelector(
  selectAnalysisState,
  (state) => state.analysis
);

export const selectAnalysisLoading = createSelector(
  selectAnalysisState,
  (state) => state.loading
);

export const selectAnalysisError = createSelector(
  selectAnalysisState,
  (state) => state.error
);

export const selectAnalysisGeneratedAt = createSelector(
  selectAnalysisState,
  (state) => state.generatedAt
);
