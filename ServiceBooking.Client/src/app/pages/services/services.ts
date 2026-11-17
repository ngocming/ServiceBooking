import { Component, inject, OnInit, signal, computed } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { DecimalPipe } from '@angular/common';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { ProviderServices, ProviderServiceItem, ServiceFilter } from '../../services/provider_services/provider-services';
import { CategoryService, CategoryItem } from '../../services/category/category';

@Component({
  selector: 'app-services',
  imports: [RouterLink, FormsModule, DecimalPipe],
  templateUrl: './services.html',
  styleUrl: './services.css',
})
export class Services implements OnInit {
  services = signal<ProviderServiceItem[]>([]);
  categories = signal<CategoryItem[]>([]);
  selectedCategoryId = signal<number | null>(null);

  loading = signal(false);
  errorMessage = signal('');
  deletingId = signal<number | null>(null);

  // Filters
  searchQuery = signal('');
  priceFilter = signal('all');
  sortOption = signal('newest');

  // Pagination
  pageIndex = signal(1);
  pageSize = signal(9);
  totalCount = signal(0);
  totalPages = signal(1);
  hasPreviousPage = signal(false);
  hasNextPage = signal(false);

  pagesArray = computed(() => {
    const total = this.totalPages();
    const current = this.pageIndex();
    const pages: number[] = [];
    for (let i = 1; i <= total; i++) {
      pages.push(i);
    }
    return pages;
  });

  userRole = '';

  private providerService = inject(ProviderServices);
  private categoryService = inject(CategoryService);
  private router = inject(Router);

  private searchSubject = new Subject<string>();

  ngOnInit(): void {
    const raw = localStorage.getItem('user');
    this.userRole = raw ? JSON.parse(raw).role : '';

    this.loadCategories();
    this.loadServices();

    // Debounce search input
    this.searchSubject.pipe(
      debounceTime(400),
      distinctUntilChanged()
    ).subscribe(query => {
      this.searchQuery.set(query);
      this.pageIndex.set(1);
      this.loadServices();
    });
  }

  loadCategories(): void {
    this.categoryService.getCategories(true).subscribe({
      next: (cats) => {
        this.categories.set(cats);
      },
      error: (err) => {
        console.error('Error loading categories:', err);
      }
    });
  }

  loadServices(): void {
    this.loading.set(true);
    this.errorMessage.set('');

    const filter: ServiceFilter = {
      pageIndex: this.pageIndex(),
      pageSize: this.pageSize()
    };

    const q = this.searchQuery().trim();
    if (q) filter.search = q;

    if (this.selectedCategoryId() !== null) {
      filter.categoryId = this.selectedCategoryId()!;
    }

    const pf = this.priceFilter();
    if (pf === 'under200k') {
      filter.maxPrice = 200000;
    } else if (pf === '200k-500k') {
      filter.minPrice = 200000;
      filter.maxPrice = 500000;
    } else if (pf === '500k-2m') {
      filter.minPrice = 500000;
      filter.maxPrice = 2000000;
    } else if (pf === 'over2m') {
      filter.minPrice = 2000000;
    }

    const sort = this.sortOption();
    if (sort === 'price_asc') {
      filter.sortBy = 'price';
      filter.sortDescending = false;
    } else if (sort === 'price_desc') {
      filter.sortBy = 'price';
      filter.sortDescending = true;
    } else if (sort === 'duration_asc') {
      filter.sortBy = 'duration';
      filter.sortDescending = false;
    } else {
      filter.sortBy = 'createdAt';
      filter.sortDescending = true;
    }

    this.providerService.getAllProviderServices(filter).subscribe({
      next: (result) => {
        this.services.set(result.items);
        this.totalCount.set(result.totalCount);
        const resolvedPage = result.pageNumber ?? result.pageIndex ?? 1;
        this.pageIndex.set(resolvedPage);
        this.totalPages.set(result.totalPages);
        this.hasPreviousPage.set(result.hasPreviousPage);
        this.hasNextPage.set(result.hasNextPage);
        this.loading.set(false);
      },
      error: (error) => {
        console.error('Error loading services:', error);
        this.errorMessage.set('Không thể tải danh sách dịch vụ.');
        this.loading.set(false);
      }
    });
  }

  selectCategory(catId: number | null): void {
    if (this.selectedCategoryId() === catId) return;
    this.selectedCategoryId.set(catId);
    this.pageIndex.set(1);
    this.loadServices();
  }

  onSearchChange(val: string): void {
    this.searchSubject.next(val);
  }

  onPriceFilterChange(val: string): void {
    this.priceFilter.set(val);
    this.pageIndex.set(1);
    this.loadServices();
  }

  onSortChange(val: string): void {
    this.sortOption.set(val);
    this.pageIndex.set(1);
    this.loadServices();
  }

  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages()) return;
    this.pageIndex.set(page);
    this.loadServices();
    window.scrollTo({ top: 0, behavior: 'smooth' });
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
        this.totalCount.update(c => Math.max(0, c - 1));
        this.deletingId.set(null);
      },
      error: (err) => {
        console.error('Error deleting service:', err);
        this.deletingId.set(null);
        alert('Xóa dịch vụ thất bại. Vui lòng thử lại.');
      }
    });
  }

  getServiceDefaultImage(service: ProviderServiceItem): string {
    if (service.imageUrl) return service.imageUrl;
    const cat = (service.categoryName || '').toLowerCase();
    const name = service.name.toLowerCase();

    if (cat.includes('rửa') || name.includes('rửa') || name.includes('car wash') || name.includes('vệ sinh')) {
      return 'https://images.unsplash.com/photo-1520340356584-f9917d1eea6f?w=600&auto=format&fit=crop&q=80';
    }
    if (cat.includes('bảo dưỡng') || name.includes('bảo dưỡng') || name.includes('thay dầu') || name.includes('nhớt')) {
      return 'https://images.unsplash.com/photo-1486006920555-c77dce18193b?w=600&auto=format&fit=crop&q=80';
    }
    if (cat.includes('sơn') || name.includes('sơn') || name.includes('đánh bóng') || name.includes('phủ ceramic')) {
      return 'https://images.unsplash.com/photo-1601362840469-51e4d8d58785?w=600&auto=format&fit=crop&q=80';
    }
    if (cat.includes('máy') || name.includes('sửa chữa') || name.includes('phanh') || name.includes('lốp')) {
      return 'https://images.unsplash.com/photo-1487754180451-c456f719a1fc?w=600&auto=format&fit=crop&q=80';
    }
    if (cat.includes('độ') || name.includes('nội thất') || name.includes('phim cách nhiệt') || name.includes('loa')) {
      return 'https://images.unsplash.com/photo-1507136566006-cfc505b114fc?w=600&auto=format&fit=crop&q=80';
    }
    return 'https://images.unsplash.com/photo-1617814076367-b759c7d7e738?w=600&auto=format&fit=crop&q=80';
  }
}
