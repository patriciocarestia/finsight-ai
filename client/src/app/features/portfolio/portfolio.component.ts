import { Component, OnInit, inject, signal } from '@angular/core';
import { Store } from '@ngrx/store';
import { AsyncPipe, DecimalPipe, DatePipe } from '@angular/common';
import { AddPositionComponent } from './add-position/add-position.component';
import {
  loadPositions,
  deletePosition,
  addPositionSuccess,
} from '../../store/portfolio/portfolio.actions';
import {
  selectPositions,
  selectPortfolioLoading,
  selectPortfolioError,
} from '../../store/portfolio/portfolio.selectors';
import { loadRates } from '../../store/rates/rates.actions';
import { Actions, ofType } from '@ngrx/effects';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

const ASSET_LABELS: Record<string, string> = {
  ARS: 'Pesos',
  USD_OFICIAL: 'USD Oficial',
  USD_BLUE: 'USD Blue',
  USDT: 'USDT',
  BTC: 'Bitcoin',
  ETH: 'Ethereum',
  PLAZO_FIJO: 'Plazo Fijo',
};

@Component({
  selector: 'app-portfolio',
  imports: [AsyncPipe, DecimalPipe, DatePipe, AddPositionComponent],
  templateUrl: './portfolio.component.html',
})
export class PortfolioComponent implements OnInit {
  private readonly store = inject(Store);
  private readonly actions$ = inject(Actions);

  readonly positions$ = this.store.select(selectPositions);
  readonly loading$ = this.store.select(selectPortfolioLoading);
  readonly error$ = this.store.select(selectPortfolioError);
  readonly showAdd = signal(false);

  constructor() {
    this.actions$
      .pipe(ofType(addPositionSuccess), takeUntilDestroyed())
      .subscribe(() => this.showAdd.set(false));
  }

  ngOnInit() {
    this.store.dispatch(loadPositions());
    this.store.dispatch(loadRates());
  }

  onDelete(id: number) {
    if (confirm('¿Eliminar esta posición?')) {
      this.store.dispatch(deletePosition({ id }));
    }
  }

  getAssetLabel(type: string): string {
    return ASSET_LABELS[type] ?? type;
  }

  getTotalInvested(
    positions: { amount: number; purchasePrice: number }[],
  ): number {
    return positions.reduce((sum, p) => sum + p.amount * p.purchasePrice, 0);
  }

  getUniqueAssets(positions: { assetType: string }[]): number {
    return new Set(positions.map((p) => p.assetType)).size;
  }
}
