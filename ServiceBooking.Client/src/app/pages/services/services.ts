import { Component, inject, OnInit, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { DecimalPipe } from '@angular/common';
import { ProviderServices, ProviderServiceItem } from '../../services/provider_services/provider-services';

@Component({
  selector: 'app-services',
  imports: [RouterLink, DecimalPipe],
  templateUrl: './services.html',
  styleUrl: './services.css',
})
export class Services implements OnInit {
  services = signal<ProviderServiceItem[]>([]);
  loading = signal(false);
  errorMessage = signal('');
  deletingId = signal<number | null>(null);

  userRole = '';

  private providerService = inject(ProviderServices);
  private router = inject(Router);

  ngOnInit(): void {
    const raw = localStorage.getItem('user');
    this.userRole = raw ? JSON.parse(raw).role : '';
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

  navigateToCreate(): void {
    this.router.navigate(['/provider/services/create']);
  }

  deleteService(id: number, event: Event): void {
    event.stopPropagation();
    event.preventDefault();
    if (!confirm('Bạn có chắc muốn xóa dịch vụ này?')) return;
    this.deletingId.set(id);
    this.providerService.deleteProviderService(id).subscribe({
      next: () => {
        this.services.update(list => list.filter(s => s.id !== id));
        this.deletingId.set(null);
      },
      error: (err) => {
        console.error('Error deleting service:', err);
        this.deletingId.set(null);
        alert('Xóa dịch vụ thất bại. Vui lòng thử lại.');
      }
    });
  }
}