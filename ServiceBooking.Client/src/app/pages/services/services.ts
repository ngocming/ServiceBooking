import { Component, inject, OnInit, signal } from '@angular/core';
import { ProviderServices, ProviderServiceItem } from '../../services/provider_services/provider-services';
import { RouterLink } from '@angular/router';
@Component({
  imports: [RouterLink],
  selector: 'app-services',
  styleUrl: './services.css',
  templateUrl: './services.html',
})
export class Services implements OnInit {
  services = signal<ProviderServiceItem[]>([]);
  private providerService = inject(ProviderServices);

  ngOnInit(): void {
    this.loadServices();
  }

  loadServices() {
    console.log('Loading services from API...');
    this.providerService.getAllProviderServices().subscribe({
      next: (data: ProviderServiceItem[]) => {
        console.log('Services received from API:', data);
        this.services.set(data || []);
      },
      error: (error) => {
        console.error('Error loading services:', error);
      }
    });
  }
  deleteService(id: number) {
    this.providerService.deleteProviderService(id).subscribe({
      next: () => {
        this.loadServices();
      },
      error: (error) => {
        console.error('Error deleting service:', error);
      }
    });
  }

}
