import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { environment } from '../../../environments/environment';

export interface AuthRequest {
  email: string;
  password: string;
}

export interface AuthResponse {
  token: string;
  email: string;
  expiresAt: string;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/auth`;

  login(request: AuthRequest) {
    return this.http.post<AuthResponse>(`${this.baseUrl}/login`, request);
  }

  register(request: AuthRequest) {
    return this.http.post<AuthResponse>(`${this.baseUrl}/register`, request);
  }
}
