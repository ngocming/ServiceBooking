import { Component, Input, Output, EventEmitter, inject } from '@angular/core';
import { DatePipe, LowerCasePipe, DecimalPipe } from '@angular/common';
import { Booking, BookingItem, BookingStatusHistoryItem } from '../../services/booking/booking';

@Component({
  imports: [DatePipe, LowerCasePipe, DecimalPipe],
  selector: 'app-booking-card',
  styleUrl: './booking-card.css',
  templateUrl: './booking-card.html',
})
export class BookingCard {
  @Input() booking!: BookingItem;
  @Input() userRole: string = '';
  @Output() cancelBooking = new EventEmitter<number>();
  @Output() confirmBooking = new EventEmitter<number>();
  @Output() completeBooking = new EventEmitter<number>();
  @Output() rejectBooking = new EventEmitter<number>();

  private bookingService = inject(Booking);

  showHistory = false;
  loadingHistory = false;
  historyList: BookingStatusHistoryItem[] = [];

  toggleHistory() {
    this.showHistory = !this.showHistory;
    if (this.showHistory && this.historyList.length === 0) {
      this.loadingHistory = true;
      this.bookingService.getBookingHistory(this.booking.id).subscribe({
        next: (res) => {
          this.historyList = res;
          this.loadingHistory = false;
        },
        error: () => {
          this.loadingHistory = false;
        }
      });
    }
  }

  onCancel() {
    this.cancelBooking.emit(this.booking.id);
  }
  onConfirm() {
    this.confirmBooking.emit(this.booking.id);
  }
  onComplete() {
    this.completeBooking.emit(this.booking.id);
  }
  onReject() {
    this.rejectBooking.emit(this.booking.id);
  }
}
