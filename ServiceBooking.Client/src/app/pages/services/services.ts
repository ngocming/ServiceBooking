import { Component, inject, OnInit, signal } from '@angular/core';
import { ProviderServices, ProviderServiceItem } from '../../services/provider_services/provider-services';
import { RouterLink } from '@angular/router';
import { Provider } from '../../services/provider/provider';
import { switchMap } from 'rxjs';
@Component({
  imports: [RouterLink],
  selector: 'app-services',
  styleUrl: './services.css',
  templateUrl: './services.html',
})
export class Services implements OnInit {
  services = signal<ProviderServiceItem[]>([]);
  private providerService = inject(ProviderServices);
  private provider = inject(Provider);
  ngOnInit(): void {
    this.loadServices();
  }

  loadServices() {
    console.log('Loading services from API...');
    this.provider.getMyProfile()
      .pipe(
        switchMap(provider =>
          this.providerService.getProviderServicesByProviderId(provider.id)
        )
      )
      .subscribe({
        next: services => {
          console.log('Services received from API:', services);
          this.services.set(services);
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
