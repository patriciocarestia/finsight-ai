import { createAction, props } from '@ngrx/store';

export const analyzePortfolio = createAction('[Analysis] Analyze Portfolio');

export const analyzePortfolioSuccess = createAction(
  '[Analysis] Analyze Portfolio Success',
  props<{ analysis: string; generatedAt: string }>(),
);

export const analyzePortfolioFailure = createAction(
  '[Analysis] Analyze Portfolio Failure',
  props<{ error: string }>(),
);
