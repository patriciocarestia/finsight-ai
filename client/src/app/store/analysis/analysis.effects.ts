import { Injectable, inject } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { catchError, map, switchMap } from 'rxjs/operators';
import { AnalysisService } from '../../core/services/analysis.service';
import * as AnalysisActions from './analysis.actions';

@Injectable()
export class AnalysisEffects {
  private readonly actions$ = inject(Actions);
  private readonly analysisService = inject(AnalysisService);

  analyze$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AnalysisActions.analyzePortfolio),
      switchMap(() =>
        this.analysisService.analyze().pipe(
          map((response) =>
            AnalysisActions.analyzePortfolioSuccess({
              analysis: response.analysis,
              generatedAt: response.generatedAt,
            }),
          ),
          catchError((err) =>
            of(
              AnalysisActions.analyzePortfolioFailure({
                error: err.error?.error ?? 'Analysis failed. Please try again.',
              }),
            ),
          ),
        ),
      ),
    ),
  );
}
