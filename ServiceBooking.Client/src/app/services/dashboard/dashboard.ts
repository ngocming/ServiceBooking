import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface ProviderDashboardStats {
  totalServices: number;
  pendingBookings: number;
  confirmedBookings: number;
  completedBookings: number;
  totalRevenue: number;
}

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  private baseUrl = `${environment.apiUrl}/dashboard`;
  private http = inject(HttpClient);

  getProviderDashboard(): Observable<ProviderDashboardStats> {
    return this.http.get<ProviderDashboardStats>(`${this.baseUrl}/provider`);
  }
}
