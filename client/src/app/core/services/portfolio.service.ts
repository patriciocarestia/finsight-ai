import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { environment } from '../../../environments/environment';
import { Position, AddPositionRequest } from '../../store/portfolio/portfolio.model';

@Injectable({ providedIn: 'root' })
export class PortfolioService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/portfolio`;

  getPositions() {
    return this.http.get<Position[]>(this.baseUrl);
  }

  addPosition(position: AddPositionRequest) {
    return this.http.post<Position>(this.baseUrl, position);
  }

  deletePosition(id: number) {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
