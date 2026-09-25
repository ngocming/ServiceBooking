import { Service, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface ProviderItem {
    id: number;
    providerId: number;
    name: string;
    description: string;
    price: number;
    durationMinutes: number;
    createdAt: string;
    isAvailable: boolean;
}
@Service()
export class Provider {

    private baseUrl = 'http://localhost:5128/api/providers';
    private http = inject(HttpClient);
    getProfile(): Observable<ProviderItem[]> {
        return this.http.get<ProviderItem[]>(`${this.baseUrl}/profile`);
    }

    getAll(): Observable<ProviderItem[]> {
        return this.http.get<ProviderItem[]>(this.baseUrl);
    }
    getById(id: number): Observable<ProviderItem> {
        return this.http.get<ProviderItem>(`${this.baseUrl}/${id}`);
    }
    add(body: any) {
        return this.http.post(this.baseUrl, body);
    }
    update(id: number, body: any) {
        return this.http.put(`${this.baseUrl}/${id}`, body);
    }

}
