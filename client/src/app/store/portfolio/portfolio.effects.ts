import { Injectable, inject } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { catchError, map, switchMap } from 'rxjs/operators';
import { PortfolioService } from '../../core/services/portfolio.service';
import * as PortfolioActions from './portfolio.actions';

@Injectable()
export class PortfolioEffects {
  private readonly actions$ = inject(Actions);
  private readonly portfolioService = inject(PortfolioService);

  loadPositions$ = createEffect(() =>
    this.actions$.pipe(
      ofType(PortfolioActions.loadPositions),
      switchMap(() =>
        this.portfolioService.getPositions().pipe(
          map((positions) => PortfolioActions.loadPositionsSuccess({ positions })),
          catchError((err) =>
            of(
              PortfolioActions.loadPositionsFailure({
                error: err.error?.error ?? 'Failed to load positions.',
              }),
            ),
          ),
        ),
      ),
    ),
  );

  addPosition$ = createEffect(() =>
    this.actions$.pipe(
      ofType(PortfolioActions.addPosition),
      switchMap(({ position }) =>
        this.portfolioService.addPosition(position).pipe(
          map((created) => PortfolioActions.addPositionSuccess({ position: created })),
          catchError((err) =>
            of(
              PortfolioActions.addPositionFailure({
                error: err.error?.error ?? 'Failed to add position.',
              }),
            ),
          ),
        ),
      ),
    ),
  );

  deletePosition$ = createEffect(() =>
    this.actions$.pipe(
      ofType(PortfolioActions.deletePosition),
      switchMap(({ id }) =>
        this.portfolioService.deletePosition(id).pipe(
          map(() => PortfolioActions.deletePositionSuccess({ id })),
          catchError((err) =>
            of(
              PortfolioActions.deletePositionFailure({
                error: err.error?.error ?? 'Failed to delete position.',
              }),
            ),
          ),
        ),
      ),
    ),
  );
}
