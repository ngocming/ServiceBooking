import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface ProviderProfile {
  id: number;
  userId: number;
  displayName: string;
  description: string;
  phone: string;
  address: string;
  latitude: number;
  longitude: number;
  isAvailable: boolean;
  createdAt: string;
}

export interface UpdateProviderDto {
  displayName?: string;
  description?: string;
  phone?: string;
  address?: string;
  latitude?: number;
  longitude?: number;
  isAvailable?: boolean;
}

export interface ProviderScheduleItem {
  dayOfWeek: number;
  dayName: string;
  isAvailable: boolean;
  startTime: string;
  endTime: string;
  slotDurationMinutes: number;
}

export interface ProviderOfferingItem {
  id: number;
  name: string;
  description: string;
  price: number;
  durationMinutes: number;
  categoryId?: number;
  categoryName?: string;
  imageUrl?: string;
  isAvailable: boolean;
}

export interface ProviderPublicProfile {
  id: number;
  displayName: string;
  description?: string;
  phone?: string;
  address?: string;
  isAvailable: boolean;
  createdAt: string;
  completedBookingsCount: number;
  services: ProviderOfferingItem[];
  weeklySchedule: ProviderScheduleItem[];
}

@Injectable({
  providedIn: 'root'
})
export class Provider {
  private baseUrl = `${environment.apiUrl}/providers`;
  private http = inject(HttpClient);

  getMyProfile(): Observable<ProviderProfile> {
    return this.http.get<ProviderProfile>(`${this.baseUrl}/profile`);
  }

  updateMyProfile(dto: UpdateProviderDto): Observable<ProviderProfile> {
    return this.http.put<ProviderProfile>(`${this.baseUrl}/profile`, dto);
  }

  getPublicProfile(id: number): Observable<ProviderPublicProfile> {
    return this.http.get<ProviderPublicProfile>(`${this.baseUrl}/${id}/public`);
  }
}
