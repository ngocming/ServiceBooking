import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface ProviderServiceItem {
  id: number;
  providerId: number;
  providerName?: string;
  categoryId?: number;
  categoryName?: string;
  name: string;
  description: string;
  imageUrl?: string;
  price: number;
  durationMinutes: number;
  createdAt: string;
  isAvailable: boolean;
}

export interface ServiceFilter {
  search?: string;
  categoryId?: number;
  minPrice?: number;
  maxPrice?: number;
  sortBy?: string;
  sortDescending?: boolean;
  pageNumber?: number;
  pageIndex?: number;
  pageSize?: number;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageIndex?: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

export interface CreateProviderServiceDTO {
  name: string;
  description: string;
  price: number;
  durationMinutes: number;
  categoryId?: number;
  imageUrl?: string;
}

@Injectable({
  providedIn: 'root'
})
export class ProviderServices {
  private baseUrl = `${environment.apiUrl}/pvdservices`;
  private http = inject(HttpClient);

  getAllProviderServices(filter?: ServiceFilter): Observable<PagedResult<ProviderServiceItem>> {
    let params = new HttpParams();
    if (filter) {
      if (filter.search) params = params.set('search', filter.search);
      if (filter.categoryId != null) params = params.set('categoryId', filter.categoryId.toString());
      if (filter.minPrice != null) params = params.set('minPrice', filter.minPrice.toString());
      if (filter.maxPrice != null) params = params.set('maxPrice', filter.maxPrice.toString());
      if (filter.sortBy) params = params.set('sortBy', filter.sortBy);
      if (filter.sortDescending != null) params = params.set('sortDescending', filter.sortDescending.toString());
      const page = filter.pageNumber ?? filter.pageIndex;
      if (page != null) {
        params = params.set('pageNumber', page.toString());
        params = params.set('pageIndex', page.toString());
      }
      if (filter.pageSize != null) params = params.set('pageSize', filter.pageSize.toString());
    }
    return this.http.get<PagedResult<ProviderServiceItem>>(this.baseUrl, { params });
  }

  getProviderServiceById(id: number): Observable<ProviderServiceItem> {
    return this.http.get<ProviderServiceItem>(`${this.baseUrl}/${id}`);
  }

  getProviderServicesByProviderId(providerId: number): Observable<ProviderServiceItem[]> {
    return this.http.get<ProviderServiceItem[]>(`${this.baseUrl}/provider/${providerId}`);
  }

  createProviderService(
    serviceData: CreateProviderServiceDTO
  ): Observable<ProviderServiceItem> {
    return this.http.post<ProviderServiceItem>(this.baseUrl, serviceData);
  }

  deleteProviderService(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
