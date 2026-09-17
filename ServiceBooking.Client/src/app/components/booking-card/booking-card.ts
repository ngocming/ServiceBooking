import { Component, Input, Output, EventEmitter } from '@angular/core';
import { DatePipe } from '@angular/common';
import { BookingItem } from '../../services/bookingservice/booking';

@Component({
  imports: [DatePipe],
  selector: 'app-booking-card',
  styleUrl: './booking-card.css',
  templateUrl: './booking-card.html',
})
export class BookingCard {
  @Input() booking!: BookingItem;
  @Output() cancelBooking = new EventEmitter<number>();

  onCancel() {
    this.cancelBooking.emit(this.booking.id);
  }
}
