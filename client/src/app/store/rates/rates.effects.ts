import { Injectable, inject } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { catchError, map, switchMap } from 'rxjs/operators';
import { RatesService } from '../../core/services/rates.service';
import * as RatesActions from './rates.actions';

@Injectable()
export class RatesEffects {
  private readonly actions$ = inject(Actions);
  private readonly ratesService = inject(RatesService);

  loadRates$ = createEffect(() =>
    this.actions$.pipe(
      ofType(RatesActions.loadRates),
      switchMap(() =>
        this.ratesService.getLatest().pipe(
          map((response) =>
            RatesActions.loadRatesSuccess({
              exchangeRates: response.exchangeRates,
              cryptoRates: response.cryptoRates,
            }),
          ),
          catchError((err) =>
            of(
              RatesActions.loadRatesFailure({
                error: err.error?.error ?? 'Failed to load rates.',
              }),
            ),
          ),
        ),
      ),
    ),
  );

  loadHistory$ = createEffect(() =>
    this.actions$.pipe(
      ofType(RatesActions.loadHistory),
      switchMap(({ rateType, days }) =>
        this.ratesService.getHistory(rateType, days).pipe(
          map((history) => RatesActions.loadHistorySuccess({ history })),
          catchError((err) =>
            of(
              RatesActions.loadHistoryFailure({
                error: err.error?.error ?? 'Failed to load history.',
              }),
            ),
          ),
        ),
      ),
    ),
  );
}
