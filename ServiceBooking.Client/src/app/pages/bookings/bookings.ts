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
  loading = false;
  errorMessage = '';
  Message = '';
  ngOnInit() {
    this.loadBookings();
  }

  loadBookings() {
    this.loading = true;
    this.errorMessage = '';
    
    this.bookingService.getBooking().subscribe({
      next: (bookings) => {
        this.bookings = bookings;
        this.loading = false;
        this.Message = 'Danh sách booking đã được tải thành công.';
      },
      error: (err) => {
        console.error('Error loading bookings:', err);
        if (err.status === 401) {
          this.errorMessage = 'Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.';
        } else if (err.status === 403) {
          this.errorMessage = 'Tài khoản này không có quyền xem danh sách booking này.';
        } else {
          this.errorMessage = 'Không tải được danh sách booking. Vui lòng thử lại sau.';
        }
        this.loading = false;
      }
    });
  }
  onCancel(id: number) {
    this.bookingService.cancelBooking(id).subscribe({
      next: () => {
        this.bookings = this.bookings.filter(b => b.id !== id);
      },
      error: (err) => {
        console.error('Error cancelling booking:', err);
      }
    });
  }
}
