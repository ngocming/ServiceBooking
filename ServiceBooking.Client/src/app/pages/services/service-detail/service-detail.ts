import { Component, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CurrencyPipe, DatePipe } from '@angular/common';
import {
  ProviderServices,
  ProviderServiceItem
} from '../../../services/provider_services/provider-services';
import { Provider, ProviderPublicProfile } from '../../../services/provider/provider';
import { Booking } from '../../../services/booking/booking';

@Component({
  selector: 'app-service-detail',
  imports: [ReactiveFormsModule, CurrencyPipe, DatePipe, RouterLink],
  templateUrl: './service-detail.html',
  styleUrl: './service-detail.css'
})
export class ServiceDetail implements OnInit {

  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private providerServices = inject(ProviderServices);
  private providerProfileService = inject(Provider);
  private bookingService = inject(Booking);

  service = signal<ProviderServiceItem | null>(null);
  providerProfile = signal<ProviderPublicProfile | null>(null);

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

    this.route.paramMap.subscribe(params => {
      const id = params.get('id');
      if (id) {
        this.loadService(Number(id));
      }
    });
  }

  loadService(id: number): void {
    this.loading.set(true);
    this.errorMessage.set('');

    this.providerServices.getProviderServiceById(id).subscribe({
      next: (data) => {
        this.service.set(data);
        this.loading.set(false);
        if (data.providerId) {
          this.loadProviderProfile(data.providerId);
        }
      },
      error: (error) => {
        console.error(error);
        this.errorMessage.set('Không thể tải thông tin dịch vụ.');
        this.loading.set(false);
      }
    });
  }

  loadProviderProfile(providerId: number): void {
    this.providerProfileService.getPublicProfile(providerId).subscribe({
      next: (profile) => {
        this.providerProfile.set(profile);
      },
      error: (err) => {
        console.warn('Could not load public provider profile:', err);
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
          this.bookingError.set('Khung giờ này đã bị trùng lịch hoặc gara không hoạt động. Vui lòng chọn thời gian khác.');
        } else if (err.status === 400) {
          this.bookingError.set('Thông tin đặt lịch không hợp lệ hoặc nằm ngoài lịch làm việc của gara.');
        } else {
          this.bookingError.set('Đặt lịch thất bại. Vui lòng thử lại sau.');
        }
      }
    });
  }

  getServiceDefaultImage(service: ProviderServiceItem): string {
    if (service.imageUrl) return service.imageUrl;
    const cat = (service.categoryName || '').toLowerCase();
    const name = service.name.toLowerCase();

    if (cat.includes('rửa') || name.includes('rửa') || name.includes('car wash') || name.includes('vệ sinh')) {
      return 'https://images.unsplash.com/photo-1520340356584-f9917d1eea6f?w=900&auto=format&fit=crop&q=80';
    }
    if (cat.includes('bảo dưỡng') || name.includes('bảo dưỡng') || name.includes('thay dầu') || name.includes('nhớt')) {
      return 'https://images.unsplash.com/photo-1486006920555-c77dce18193b?w=900&auto=format&fit=crop&q=80';
    }
    if (cat.includes('sơn') || name.includes('sơn') || name.includes('đánh bóng') || name.includes('phủ ceramic')) {
      return 'https://images.unsplash.com/photo-1601362840469-51e4d8d58785?w=900&auto=format&fit=crop&q=80';
    }
    if (cat.includes('máy') || name.includes('sửa chữa') || name.includes('phanh') || name.includes('lốp')) {
      return 'https://images.unsplash.com/photo-1487754180451-c456f719a1fc?w=900&auto=format&fit=crop&q=80';
    }
    return 'https://images.unsplash.com/photo-1617814076367-b759c7d7e738?w=900&auto=format&fit=crop&q=80';
  }
}
