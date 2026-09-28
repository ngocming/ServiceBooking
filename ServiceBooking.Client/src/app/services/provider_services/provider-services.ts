import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface ProviderServiceItem {
  id: number;
  providerId: number;
  name: string;
  description: string;
  price: number;
  durationMinutes: number;
  createdAt: string;
  isAvailable: boolean;
}

export interface CreateProviderServiceDTO {
  name: string;
  description: string;
  price: number;
  durationMinutes: number;
}

@Injectable({
  providedIn: 'root'
})
export class ProviderServices {
  private baseUrl = 'http://localhost:5128/api/pvdservices';
  private http = inject(HttpClient);

  getAllProviderServices(): Observable<ProviderServiceItem[]> {
    return this.http.get<ProviderServiceItem[]>(this.baseUrl);
  }

  getProviderServiceById(id: number): Observable<ProviderServiceItem> {
    return this.http.get<ProviderServiceItem>(`${this.baseUrl}/${id}`);
  }

  getProviderServicesByProviderId(id: number): Observable<ProviderServiceItem[]> {
    return this.http.get<ProviderServiceItem[]>(`${this.baseUrl}/provider/${id}`);
  }

  createProviderService(
    serviceData: CreateProviderServiceDTO
  ): Observable<ProviderServiceItem> {
    return this.http.post<ProviderServiceItem>(this.baseUrl, serviceData);
  }

  deleteProviderService(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
