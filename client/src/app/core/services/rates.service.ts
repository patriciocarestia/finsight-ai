import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { environment } from '../../../environments/environment';
import { LatestRatesResponse, ExchangeRate } from '../../store/rates/rates.model';

@Injectable({ providedIn: 'root' })
export class RatesService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/rates`;

  getLatest() {
    return this.http.get<LatestRatesResponse>(`${this.baseUrl}/latest`);
  }

  getHistory(type: string, days: number) {
    return this.http.get<ExchangeRate[]>(`${this.baseUrl}/history`, {
      params: { type, days: days.toString() }
    });
  }
}
