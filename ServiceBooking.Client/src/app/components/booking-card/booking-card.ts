import { Component, Input, Output, EventEmitter } from '@angular/core';

@Component({
  imports: [],
  selector: 'app-booking-card',
  styleUrl: './booking-card.css',
  templateUrl: './booking-card.html',
})
export class BookingCard {
  @Input() booking: any;
  @Output() cancelBooking = new EventEmitter<number>();

  onCancel() {
    this.cancelBooking.emit(this.booking.id);
  }
}
