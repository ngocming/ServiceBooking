import { Component, OnInit, inject } from '@angular/core';
import { BookingCard } from '../../components/booking-card/booking-card';
import { Booking } from '../../services/bookingservice/booking';
import { BookingItem } from '../../services/bookingservice/booking';
@Component({
  imports: [BookingCard],
  selector: 'app-bookings',
  styleUrl: './bookings.css',
  templateUrl: './bookings.html',
})
export class Bookings implements OnInit {
  bookings: BookingItem[] = [];
  bookingService: Booking = inject(Booking);
  ngOnInit() {
    this.bookingService.getBooking().subscribe({
      next: (bookings) => {
        this.bookings = bookings;
      },
      error: (err) => console.error('Error loading bookings:', err)
    });
  }
  onCancel(id: number) {
    this.bookingService.cancelBooking(id).subscribe({
      next: () => {
        this.bookings = this.bookings.filter(b => b.id !== id);
      },
      error: (err) => console.error('Error cancelling booking:', err)
    });
  }
}
