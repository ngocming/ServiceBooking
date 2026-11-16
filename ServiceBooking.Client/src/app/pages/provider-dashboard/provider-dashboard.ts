import { Component, OnInit, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { DecimalPipe } from '@angular/common';
import { DashboardService, ProviderDashboardStats } from '../../services/dashboard/dashboard';

@Component({
  selector: 'app-provider-dashboard',
  imports: [RouterLink, DecimalPipe],
  templateUrl: './provider-dashboard.html',
  styleUrl: './provider-dashboard.css'
})
export class ProviderDashboard implements OnInit {
  private dashboardService = inject(DashboardService);

  stats = signal<ProviderDashboardStats | null>(null);
  loading = signal(true);
  errorMessage = signal('');

  ngOnInit(): void {
    this.loadStats();
  }

  loadStats(): void {
    this.loading.set(true);
    this.errorMessage.set('');

    this.dashboardService.getProviderDashboard().subscribe({
      next: (data) => {
        this.stats.set(data);
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Error fetching dashboard stats:', err);
        this.errorMessage.set('Không thể tải dữ liệu thống kê. Vui lòng thử lại sau.');
        this.loading.set(false);
      }
    });
  }
}
