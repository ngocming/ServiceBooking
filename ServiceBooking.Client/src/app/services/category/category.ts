import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface CategoryItem {
  id: number;
  name: string;
  slug: string;
  description?: string;
  iconUrl?: string;
  displayOrder: number;
  isActive: boolean;
  serviceCount: number;
}

@Injectable({
  providedIn: 'root'
})
export class CategoryService {
  private baseUrl = `${environment.apiUrl}/categories`;
  private http = inject(HttpClient);

  getCategories(activeOnly: boolean = true): Observable<CategoryItem[]> {
    return this.http.get<CategoryItem[]>(`${this.baseUrl}?activeOnly=${activeOnly}`);
  }

  getCategoryById(id: number): Observable<CategoryItem> {
    return this.http.get<CategoryItem>(`${this.baseUrl}/${id}`);
  }

  getCategoryBySlug(slug: string): Observable<CategoryItem> {
    return this.http.get<CategoryItem>(`${this.baseUrl}/slug/${slug}`);
  }
}
