import { Component, OnInit, OnDestroy, inject, signal } from '@angular/core';
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
  templateUrl: './dashboard.component.html'
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
