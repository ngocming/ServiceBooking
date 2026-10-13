import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ProviderServices, ProviderServiceItem } from '../../services/provider_services/provider-services';

@Component({
  selector: 'app-services',
  imports: [RouterLink],
  templateUrl: './services.html',
  styleUrl: './services.css',
})
export class Services implements OnInit {
  services = signal<ProviderServiceItem[]>([]);
  loading = signal(false);
  errorMessage = signal('');

  private providerService = inject(ProviderServices);

  ngOnInit(): void {
    this.loadServices();
  }

  loadServices(): void {
    this.loading.set(true);
    this.errorMessage.set('');

    this.providerService.getAllProviderServices().subscribe({
      next: (services) => {
        this.services.set(services);
        this.loading.set(false);
      },
      error: (error) => {
        console.error('Error loading services:', error);
        this.errorMessage.set('Không thể tải danh sách dịch vụ.');
        this.loading.set(false);
      }
    });
  }
}