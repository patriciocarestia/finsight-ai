import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { Store } from '@ngrx/store';
import { AsyncPipe, DecimalPipe, DatePipe } from '@angular/common';
import { AddPositionComponent } from './add-position/add-position.component';
import { loadPositions, deletePosition, addPositionSuccess } from '../../store/portfolio/portfolio.actions';
import {
  selectPositions,
  selectPortfolioLoading,
  selectPortfolioError
} from '../../store/portfolio/portfolio.selectors';
import { selectExchangeRates, selectCryptoRates } from '../../store/rates/rates.selectors';
import { loadRates } from '../../store/rates/rates.actions';
import { Position } from '../../store/portfolio/portfolio.model';
import { Actions, ofType } from '@ngrx/effects';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

const ASSET_LABELS: Record<string, string> = {
  ARS: 'Pesos', USD_OFICIAL: 'USD Oficial', USD_BLUE: 'USD Blue',
  USDT: 'USDT', BTC: 'Bitcoin', ETH: 'Ethereum', PLAZO_FIJO: 'Plazo Fijo'
};

@Component({
  selector: 'app-portfolio',
  imports: [AsyncPipe, DecimalPipe, DatePipe, AddPositionComponent],
  template: `
    <div class="space-y-6">
      <div class="flex items-center justify-between">
        <h1 class="text-3xl font-bold text-white">Mi Portfolio</h1>
        <button class="btn-primary" (click)="showAdd.set(!showAdd())">
          {{ showAdd() ? 'Cancelar' : '+ Agregar posición' }}
        </button>
      </div>

      @if (showAdd()) {
        <app-add-position (cancelled)="showAdd.set(false)" />
      }

      @if (error$ | async; as error) {
        <div class="bg-red-900/30 border border-red-700 text-red-400 rounded-lg px-4 py-3 text-sm">{{ error }}</div>
      }

      @if (loading$ | async) {
        <div class="text-center py-12 text-slate-400">Cargando portfolio...</div>
      }

      @if (positions$ | async; as positions) {
        @if (positions.length === 0 && !(loading$ | async)) {
          <div class="card text-center py-12">
            <p class="text-slate-400 text-lg">No tenés posiciones todavía</p>
            <p class="text-slate-500 text-sm mt-2">Agregá tu primera inversión para comenzar</p>
          </div>
        } @else if (positions.length > 0) {
          <div class="card overflow-x-auto">
            <table class="w-full text-sm">
              <thead>
                <tr class="text-slate-400 border-b border-slate-700">
                  <th class="text-left py-3 pr-4">Activo</th>
                  <th class="text-right py-3 pr-4">Cantidad</th>
                  <th class="text-right py-3 pr-4">Precio compra</th>
                  <th class="text-right py-3 pr-4">Total invertido</th>
                  <th class="text-right py-3 pr-4">Fecha</th>
                  <th class="text-right py-3">Acciones</th>
                </tr>
              </thead>
              <tbody>
                @for (pos of positions; track pos.id) {
                  <tr class="border-b border-slate-800 hover:bg-slate-800/30 transition-colors">
                    <td class="py-3 pr-4">
                      <span class="font-medium text-white">{{ getAssetLabel(pos.assetType) }}</span>
                      @if (pos.notes) {
                        <span class="text-slate-500 block text-xs">{{ pos.notes }}</span>
                      }
                    </td>
                    <td class="py-3 pr-4 text-right text-white">{{ pos.amount | number:'1.2-8' }}</td>
                    <td class="py-3 pr-4 text-right text-slate-300">$\{{ pos.purchasePrice | number:'1.0-0' }}</td>
                    <td class="py-3 pr-4 text-right font-medium text-white">
                      $\{{ (pos.amount * pos.purchasePrice) | number:'1.0-0' }}
                    </td>
                    <td class="py-3 pr-4 text-right text-slate-400">{{ pos.purchaseDate | date:'dd/MM/yy' }}</td>
                    <td class="py-3 text-right">
                      <button (click)="onDelete(pos.id)"
                              class="text-red-400 hover:text-red-300 transition-colors text-xs">
                        Eliminar
                      </button>
                    </td>
                  </tr>
                }
              </tbody>
            </table>
          </div>
        }
      }
    </div>
  `
})
export class PortfolioComponent implements OnInit {
  private readonly store = inject(Store);
  private readonly actions$ = inject(Actions);

  readonly positions$ = this.store.select(selectPositions);
  readonly loading$ = this.store.select(selectPortfolioLoading);
  readonly error$ = this.store.select(selectPortfolioError);
  readonly showAdd = signal(false);

  constructor() {
    this.actions$.pipe(
      ofType(addPositionSuccess),
      takeUntilDestroyed()
    ).subscribe(() => this.showAdd.set(false));
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
}
