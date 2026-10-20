import { HttpClient } from '@angular/common/http';
import { inject, Service } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface BookingItem {
    id: number;
    customerId: number;
    providerId: number;
    providerServiceId: number;
    serviceName: string;
    providerName: string;
    bookingDate: string;
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
    createBooking(dto: CreateBookingDto): Observable<BookingItem> {
        return this.http.post<BookingItem>(this.baseUrl, dto);
    }
    cancelBooking(id: number): Observable<void> {
        return this.http.patch<void>(
            `${this.baseUrl}/${id}/cancel`,
            {}
        );
    }
    confirmBooking(id: number): Observable<void> {
        return this.http.patch<void>(
            `${this.baseUrl}/${id}/confirm`,
            {}
        );
    }
    completeBooking(id: number): Observable<void> {
        return this.http.patch<void>(
            `${this.baseUrl}/${id}/complete`,
            {}
        );
    }
    getUserRole(): string {
        const user = localStorage.getItem('user');
        return user ? JSON.parse(user).role : '';
    }
}
