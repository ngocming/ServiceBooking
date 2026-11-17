import { HttpClient } from '@angular/common/http';
import { inject, Service } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface BookingStatusHistoryItem {
    id: number;
    bookingId: number;
    previousStatus?: string;
    newStatus: string;
    changedByUserId: number;
    changedByUsername: string;
    changedAt: string;
    reason?: string;
}

export interface BookingItem {
    id: number;
    customerId: number;
    providerId: number;
    providerServiceId: number;
    serviceName: string;
    providerName: string;
    bookingDate: string;
    startAt?: string;
    endAt?: string;
    status: string;
    note?: string;
    totalPrice: number;
    createdAt: string;
}

export interface CreateBookingDto {
    providerServiceId: number;
    bookingDate: string;
    note?: string;
}

@Service()
export class Booking {
    private baseUrl = `${environment.apiUrl}/booking`;

    private http = inject(HttpClient);

    getCustomerBookings(): Observable<BookingItem[]> {
        return this.http.get<BookingItem[]>(
            `${this.baseUrl}/customer/bookings`
        );
    }

    getProviderBookings(): Observable<BookingItem[]> {
        return this.http.get<BookingItem[]>(
            `${this.baseUrl}/provider/bookings`
        );
    }

    getBookingById(id: number): Observable<BookingItem> {
        return this.http.get<BookingItem>(`${this.baseUrl}/${id}`);
    }

    getBookingHistory(id: number): Observable<BookingStatusHistoryItem[]> {
        return this.http.get<BookingStatusHistoryItem[]>(`${this.baseUrl}/${id}/history`);
    }

    createBooking(dto: CreateBookingDto): Observable<BookingItem> {
        return this.http.post<BookingItem>(this.baseUrl, dto);
    }

    cancelBooking(id: number, reason?: string): Observable<void> {
        return this.http.patch<void>(
            `${this.baseUrl}/${id}/cancel`,
            reason ? { reason } : {}
        );
    }

    confirmBooking(id: number, reason?: string): Observable<void> {
        return this.http.patch<void>(
            `${this.baseUrl}/${id}/confirm`,
            reason ? { reason } : {}
        );
    }

    completeBooking(id: number, reason?: string): Observable<void> {
        return this.http.patch<void>(
            `${this.baseUrl}/${id}/complete`,
            reason ? { reason } : {}
        );
    }

    rejectBooking(id: number, reason?: string): Observable<void> {
        return this.http.patch<void>(
            `${this.baseUrl}/${id}/reject`,
            reason ? { reason } : {}
        );
    }

    getUserRole(): string {
        const user = localStorage.getItem('user');
        return user ? JSON.parse(user).role : '';
    }
}
