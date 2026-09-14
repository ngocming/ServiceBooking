import { HttpClient } from '@angular/common/http';
import { inject, Service } from '@angular/core';
import { Observable } from 'rxjs';

export interface BookingItem {
    id: number;
    customerName: string;
    serviceName: string;
    bookingDate: string;
    pickupLocation: string;
    status: string;
}

@Service()
export class Booking {
    private baseUrl = 'http://localhost:5128/api/booking';

    private http = inject(HttpClient);

    getBooking(): Observable<BookingItem[]> {
        return this.http.get<BookingItem[]>(
            `${this.baseUrl}/customer/bookings`
        );
    }
    cancelBooking(id: number) {
        return this.http.patch(
            `${this.baseUrl}/${id}/cancel`,
            {}
        );
    }
}
