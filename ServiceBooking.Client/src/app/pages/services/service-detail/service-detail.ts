import { Component, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CurrencyPipe, DatePipe } from '@angular/common';
import {
  ProviderServices,
  ProviderServiceItem
} from '../../../services/provider_services/provider-services';
import { Booking } from '../../../services/booking/booking';

@Component({
  selector: 'app-service-detail',
  imports: [ReactiveFormsModule, CurrencyPipe, DatePipe],
  templateUrl: './service-detail.html',
  styleUrl: './service-detail.css'
})
export class ServiceDetail implements OnInit {

  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private providerServices = inject(ProviderServices);
  private bookingService = inject(Booking);

  service = signal<ProviderServiceItem | null>(null);
  loading = signal(false);
  errorMessage = signal('');
  showBookingForm = signal(false);
  bookingLoading = signal(false);
  bookingSuccess = signal(false);
  bookingError = signal('');
  userRole = '';
  bookingForm = new FormGroup({
    bookingDate: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required]
    }),
    note: new FormControl('', { nonNullable: true })
  });
  get minDate(): string {
    const now = new Date();
    now.setMinutes(now.getMinutes() - now.getTimezoneOffset());
    return now.toISOString().slice(0, 16);
  }

  ngOnInit(): void {
    const raw = localStorage.getItem('user');
    this.userRole = raw ? JSON.parse(raw).role : '';

    const id = this.route.snapshot.paramMap.get('id');
    if (!id) {
      this.errorMessage.set('Không tìm thấy dịch vụ.');
      return;
    }
    this.loadService(Number(id));
  }

  loadService(id: number): void {
    this.loading.set(true);
    this.providerServices.getProviderServiceById(id).subscribe({
      next: (data) => {
        this.service.set(data);
        this.loading.set(false);
      },
      error: (error) => {
        console.error(error);
        this.errorMessage.set('Không thể tải thông tin dịch vụ.');
        this.loading.set(false);
      }
    });
  }

  openBookingForm(): void {
    this.showBookingForm.set(true);
    this.bookingSuccess.set(false);
    this.bookingError.set('');
    this.bookingForm.reset();
  }

  closeBookingForm(): void {
    this.showBookingForm.set(false);
  }

  submitBooking(): void {
    if (this.bookingForm.invalid) {
      this.bookingForm.markAllAsTouched();
      return;
    }

    const svc = this.service();
    if (!svc) return;

    this.bookingLoading.set(true);
    this.bookingError.set('');

    const dto = {
      providerServiceId: svc.id,
      bookingDate: new Date(this.bookingForm.value.bookingDate!).toISOString(),
      note: this.bookingForm.value.note || undefined
    };

    this.bookingService.createBooking(dto).subscribe({
      next: () => {
        this.bookingLoading.set(false);
        this.bookingSuccess.set(true);
        this.bookingForm.reset();
        setTimeout(() => {
          this.router.navigate(['/customer/bookings']);
        }, 2000);
      },
      error: (err) => {
        this.bookingLoading.set(false);
        console.error(err);
        if (err.status === 409) {
          this.bookingError.set('Thời gian này đã được đặt. Vui lòng chọn thời gian khác.');
        } else if (err.status === 400) {
          this.bookingError.set('Thông tin không hợp lệ. Vui lòng kiểm tra lại.');
        } else {
          this.bookingError.set('Đặt lịch thất bại. Vui lòng thử lại.');
        }
      }
    });
  }
}