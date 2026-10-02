import { Component, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import {
  ProviderServices,
  ProviderServiceItem
} from '../../../services/provider_services/provider-services';

@Component({
  selector: 'app-service-detail',
  templateUrl: './service-detail.html',
  styleUrl: './service-detail.css'
})
export class ServiceDetail implements OnInit {

  private route = inject(ActivatedRoute);
  private providerServices = inject(ProviderServices);

  service = signal<ProviderServiceItem | null>(null);

  loading = signal(false);
  errorMessage = signal('');

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');

    if (!id) {
      this.errorMessage.set('Không tìm thấy service.');
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

        this.errorMessage.set(
          'Không thể tải thông tin service.');

        this.loading.set(false);
      }
    });
  }
}