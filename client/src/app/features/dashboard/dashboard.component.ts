import { Component, OnInit, OnDestroy, inject, signal, computed } from '@angular/core';
import { Store } from '@ngrx/store';
import { AsyncPipe, DecimalPipe, DatePipe } from '@angular/common';
import { BaseChartDirective } from 'ng2-charts';
import { ChartConfiguration } from 'chart.js';
import { RateCardComponent } from '../../shared/components/rate-card/rate-card.component';
import { loadRates, loadHistory } from '../../store/rates/rates.actions';
import {
  selectExchangeRates,
  selectCryptoRates,
  selectRatesHistory,
  selectRatesLoading,
  selectLastFetched
} from '../../store/rates/rates.selectors';
import { ExchangeRate } from '../../store/rates/rates.model';

const RATE_LABELS: Record<string, string> = {
  oficial: 'Dólar Oficial',
  blue: 'Dólar Blue',
  mep: 'Dólar MEP',
  ccl: 'Dólar CCL',
  cripto: 'Dólar Cripto'
};

@Component({
  selector: 'app-dashboard',
  imports: [AsyncPipe, DecimalPipe, DatePipe, RateCardComponent, BaseChartDirective],
  template: `
    <div class="space-y-8">
      <!-- Header -->
      <div class="flex items-center justify-between">
        <div>
          <h1 class="text-3xl font-bold text-white">Dashboard</h1>
          <p class="text-slate-400 mt-1 text-sm">
            @if (lastFetched$ | async; as ts) {
              Última actualización: {{ ts | date:'HH:mm' }}
            }
          </p>
        </div>
        <button (click)="onRefresh()" class="btn-secondary flex items-center gap-2" [disabled]="loading$ | async">
          <span>&#8635;</span> Actualizar
        </button>
      </div>

      <!-- Loading -->
      @if (loading$ | async) {
        <div class="grid grid-cols-2 md:grid-cols-5 gap-4">
          @for (i of [1,2,3,4,5]; track i) {
            <div class="card animate-pulse bg-slate-800/50 h-28"></div>
          }
        </div>
      }

      <!-- Exchange Rates -->
      @if (exchangeRates$ | async; as rates) {
        @if (rates.length > 0) {
          <section>
            <h2 class="text-lg font-semibold text-slate-300 mb-4">Cotizaciones del Dólar</h2>
            <div class="grid grid-cols-2 md:grid-cols-5 gap-4">
              @for (rate of rates; track rate.id) {
                <app-rate-card
                  [label]="getRateLabel(rate.type)"
                  [buy]="rate.buy"
                  [sell]="rate.sell"
                  [change]="0" />
              }
            </div>
          </section>
        }
      }

      <!-- Crypto Rates -->
      @if (cryptoRates$ | async; as cryptos) {
        @if (cryptos.length > 0) {
          <section>
            <h2 class="text-lg font-semibold text-slate-300 mb-4">Criptomonedas</h2>
            <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
              @for (crypto of cryptos; track crypto.id) {
                <div class="card">
                  <div class="flex items-center justify-between mb-3">
                    <span class="text-slate-300 font-semibold text-lg">{{ crypto.symbol }}</span>
                    <span class="text-xs px-2 py-1 rounded-full"
                          [class]="crypto.changePercent24h >= 0 ? 'bg-green-900/30 text-green-400' : 'bg-red-900/30 text-red-400'">
                      {{ crypto.changePercent24h >= 0 ? '+' : '' }}{{ crypto.changePercent24h | number:'1.2-2' }}%
                    </span>
                  </div>
                  <div class="text-2xl font-bold text-white">
                    USD {{ crypto.priceUsd | number:'1.0-0' }}
                  </div>
                  <div class="text-slate-400 mt-1 text-sm">
                    ARS {{ crypto.priceArs | number:'1.0-0' }}
                  </div>
                </div>
              }
            </div>
          </section>
        }
      }

      <!-- Historical Chart -->
      <section class="card">
        <div class="flex items-center justify-between mb-6">
          <h2 class="text-lg font-semibold text-slate-300">Histórico</h2>
          <div class="flex items-center gap-2">
            <select (change)="onTypeChange($any($event.target).value)"
                    class="input w-auto text-sm">
              <option value="blue">Blue</option>
              <option value="oficial">Oficial</option>
              <option value="mep">MEP</option>
              <option value="ccl">CCL</option>
            </select>
            <div class="flex gap-1">
              @for (d of dayOptions; track d) {
                <button (click)="onDaysChange(d)"
                        class="px-3 py-1 rounded text-sm transition-colors"
                        [class]="selectedDays() === d ? 'bg-blue-600 text-white' : 'bg-slate-700 text-slate-400 hover:bg-slate-600'">
                  {{ d }}d
                </button>
              }
            </div>
          </div>
        </div>

        @if (history$ | async; as history) {
          @if (history.length > 0) {
            <canvas baseChart
                    [data]="buildChartData(history)"
                    [options]="chartOptions"
                    type="line">
            </canvas>
          } @else {
            <div class="text-center text-slate-500 py-12">No hay datos históricos disponibles</div>
          }
        }
      </section>
    </div>
  `
})
export class DashboardComponent implements OnInit, OnDestroy {
  private readonly store = inject(Store);

  readonly exchangeRates$ = this.store.select(selectExchangeRates);
  readonly cryptoRates$ = this.store.select(selectCryptoRates);
  readonly history$ = this.store.select(selectRatesHistory);
  readonly loading$ = this.store.select(selectRatesLoading);
  readonly lastFetched$ = this.store.select(selectLastFetched);

  readonly dayOptions = [7, 30, 90];
  readonly selectedDays = signal(30);
  selectedType = 'blue';
  private refreshInterval?: ReturnType<typeof setInterval>;

  readonly chartOptions: ChartConfiguration['options'] = {
    responsive: true,
    plugins: {
      legend: { display: false },
      tooltip: { mode: 'index', intersect: false }
    },
    scales: {
      x: {
        ticks: { color: '#94a3b8', maxTicksLimit: 8 },
        grid: { color: '#1e293b' }
      },
      y: {
        beginAtZero: false,
        grace: '12%',
        ticks: { color: '#94a3b8' },
        grid: { color: '#1e293b' }
      }
    }
  };

  ngOnInit() {
    this.store.dispatch(loadRates());
    this.loadHistory();
    this.refreshInterval = setInterval(() => this.store.dispatch(loadRates()), 5 * 60 * 1000);
  }

  ngOnDestroy() {
    if (this.refreshInterval) clearInterval(this.refreshInterval);
  }

  onRefresh() {
    this.store.dispatch(loadRates());
    this.loadHistory();
  }

  onTypeChange(type: string) {
    this.selectedType = type;
    this.loadHistory();
  }

  onDaysChange(days: number) {
    this.selectedDays.set(days);
    this.loadHistory();
  }

  getRateLabel(type: string): string {
    return RATE_LABELS[type] ?? type;
  }

  buildChartData(history: ExchangeRate[]): ChartConfiguration['data'] {
    const sorted = [...history].sort(
      (a, b) => new Date(a.recordedAt).getTime() - new Date(b.recordedAt).getTime()
    );

    // One point per calendar day — last value wins (covers today's intraday Hangfire records)
    const byDay = new Map<string, number>();
    for (const r of sorted) {
      const day = new Date(r.recordedAt).toLocaleDateString('es-AR', { day: '2-digit', month: '2-digit' });
      byDay.set(day, r.sell);
    }

    const entries = [...byDay.entries()];

    return {
      labels: entries.map(([day]) => day),
      datasets: [{
        data: entries.map(([, sell]) => sell),
        borderColor: '#3b82f6',
        backgroundColor: 'rgba(59, 130, 246, 0.1)',
        fill: true,
        tension: 0.4,
        pointRadius: 0
      }]
    };
  }

  private loadHistory() {
    this.store.dispatch(loadHistory({ rateType: this.selectedType, days: this.selectedDays() }));
  }
}
