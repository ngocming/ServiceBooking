import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ProviderServices, ProviderServiceItem } from '../../services/provider_services/provider-services';

@Component({
  selector: 'app-service-detail',
  imports: [RouterLink],
  templateUrl: './service-detail.html',
  styleUrl: './service-detail.css'
})
export class ServiceDetail implements OnInit {
  service: ProviderServiceItem | null = null;
  loading = false;
  errorMessage = '';

  private route = inject(ActivatedRoute);
  private providerServices = inject(ProviderServices);

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));

    if (!id) {
      this.errorMessage = 'ID dịch vụ không hợp lệ.';
      return;
    }

    this.loadService(id);
  }

  private loadService(id: number): void {
    this.loading = true;

    this.providerServices.getProviderServiceById(id).subscribe({
      next: (data) => {
        this.service = data;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading service:', error);
        this.errorMessage = 'Không tìm thấy dịch vụ.';
        this.loading = false;
      }
    });
  }
}