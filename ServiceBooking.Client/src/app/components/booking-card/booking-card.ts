import { Component, Input, Output, EventEmitter } from '@angular/core';
import { DatePipe, LowerCasePipe } from '@angular/common';
import { BookingItem } from '../../services/booking/booking';

@Component({
  imports: [DatePipe, LowerCasePipe],
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

  onCancel() {
    this.cancelBooking.emit(this.booking.id);
  }
  onConfirm() {
    this.confirmBooking.emit(this.booking.id);
  }
  onComplete() {
    this.completeBooking.emit(this.booking.id);
  }
}
