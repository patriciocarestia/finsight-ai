import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { environment } from '../../../environments/environment';
import { LatestRatesResponse, ExchangeRate, CryptoRate } from '../../store/rates/rates.model';

@Injectable({ providedIn: 'root' })
export class RatesService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/rates`;

  getLatest() {
    return this.http.get<LatestRatesResponse>(`${this.baseUrl}/latest`);
  }

  getHistory(type: string, days: number) {
    return this.http.get<ExchangeRate[]>(`${this.baseUrl}/history`, {
      params: { type, days: days.toString() },
    });
  }

  getCryptoHistory(symbol: string, days: number) {
    return this.http.get<CryptoRate[]>(`${this.baseUrl}/crypto-history`, {
      params: { symbol, days: days.toString() },
    });
  }
}
