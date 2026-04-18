import { createSelector } from '@ngrx/store';
import { portfolioFeature } from './portfolio.reducer';

export const selectPortfolioState = portfolioFeature.selectPortfolioState;

export const selectPositions = createSelector(selectPortfolioState, (state) => state.positions);

export const selectPortfolioLoading = createSelector(
  selectPortfolioState,
  (state) => state.loading,
);

export const selectPortfolioSaving = createSelector(selectPortfolioState, (state) => state.saving);

export const selectPortfolioError = createSelector(selectPortfolioState, (state) => state.error);
