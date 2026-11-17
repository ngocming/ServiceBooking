import { HttpClient } from '@angular/common/http';
import { inject, Service } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface ProviderAvailabilityItem {
    id: number;
    dayOfWeek: number;
    dayName: string;
    startTime: string;
    endTime: string;
    isAvailable: boolean;
}

export interface AvailabilityExceptionItem {
    id: number;
    date: string;
    startTime?: string;
    endTime?: string;
    isAvailable: boolean;
    reason?: string;
}

export interface ProviderScheduleResponse {
    providerId: number;
    providerName: string;
    weeklySchedule: ProviderAvailabilityItem[];
    exceptions: AvailabilityExceptionItem[];
}

export interface WeeklySlotDto {
    dayOfWeek: number;
    startTime: string;
    endTime: string;
    isAvailable: boolean;
}

export interface SetWeeklyAvailabilityDto {
    slots: WeeklySlotDto[];
}

export interface CreateAvailabilityExceptionDto {
    date: string;
    startTime?: string;
    endTime?: string;
    isAvailable: boolean;
    reason?: string;
}

@Service()
export class Availability {
    private baseUrl = `${environment.apiUrl}/availability`;
    private http = inject(HttpClient);

    getProviderSchedule(providerId: number): Observable<ProviderScheduleResponse> {
        return this.http.get<ProviderScheduleResponse>(`${this.baseUrl}/providers/${providerId}`);
    }

    getMySchedule(): Observable<ProviderScheduleResponse> {
        return this.http.get<ProviderScheduleResponse>(`${this.baseUrl}/my-schedule`);
    }

    setWeeklySchedule(dto: SetWeeklyAvailabilityDto): Observable<{ message: string }> {
        return this.http.put<{ message: string }>(`${this.baseUrl}/my-schedule`, dto);
    }

    addException(dto: CreateAvailabilityExceptionDto): Observable<AvailabilityExceptionItem> {
        return this.http.post<AvailabilityExceptionItem>(`${this.baseUrl}/exceptions`, dto);
    }

    deleteException(id: number): Observable<{ message: string }> {
        return this.http.delete<{ message: string }>(`${this.baseUrl}/exceptions/${id}`);
    }
}
