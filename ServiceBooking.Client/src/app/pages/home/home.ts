import { Component, OnInit, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { DecimalPipe } from '@angular/common';
import { CategoryService, CategoryItem } from '../../services/category/category';
import { ProviderServices, ProviderServiceItem } from '../../services/provider_services/provider-services';

@Component({
  selector: 'app-home',
  imports: [RouterLink, DecimalPipe],
  templateUrl: './home.html',
  styleUrl: './home.css',
})
export class Home implements OnInit {
  private categoryService = inject(CategoryService);
  private providerServices = inject(ProviderServices);
  private router = inject(Router);

  categories = signal<CategoryItem[]>([]);
  featuredServices = signal<ProviderServiceItem[]>([]);
  userRole = '';
  username = '';

  ngOnInit(): void {
    const raw = localStorage.getItem('user');
    if (raw) {
      const user = JSON.parse(raw);
      this.userRole = user.role || '';
      this.username = user.username || '';
    }

    this.categoryService.getCategories(true).subscribe({
      next: (cats) => this.categories.set(cats.slice(0, 6)),
      error: (err) => console.warn('Could not load categories for home:', err)
    });

    this.providerServices.getAllProviderServices({ pageSize: 3, sortBy: 'createdAt', sortDescending: true }).subscribe({
      next: (res) => this.featuredServices.set(res.items),
      error: (err) => console.warn('Could not load featured services:', err)
    });
  }

  getServiceDefaultImage(service: ProviderServiceItem): string {
    if (service.imageUrl) return service.imageUrl;
    return 'https://images.unsplash.com/photo-1520340356584-f9917d1eea6f?w=800&auto=format&fit=crop&q=80';
  }
}
