import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { environment } from '../../../environments/environment';

export interface AnalysisResponse {
  analysis: string;
  generatedAt: string;
}

@Injectable({ providedIn: 'root' })
export class AnalysisService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/analysis`;

  analyze() {
    return this.http.post<AnalysisResponse>(this.baseUrl, {});
  }
}
