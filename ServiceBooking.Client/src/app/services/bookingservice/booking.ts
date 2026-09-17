import { HttpClient } from '@angular/common/http';
import { inject, Service } from '@angular/core';
import { Observable } from 'rxjs';

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

@Service()
export class Booking {
    private baseUrl = 'http://localhost:5128/api/booking';

    private http = inject(HttpClient);

    getBooking(): Observable<BookingItem[]> {
        const user = localStorage.getItem('user');
        const role = user ? JSON.parse(user).role : '';
        const path = role === 'Provider'
            ? 'provider/bookings'
            : 'customer/bookings';

        return this.http.get<BookingItem[]>(
            `${this.baseUrl}/${path}`
        );
    }
    cancelBooking(id: number) {
        return this.http.patch(
            `${this.baseUrl}/${id}/cancel`,
            {}
        );
    }
}
