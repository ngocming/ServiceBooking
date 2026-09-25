import { Service, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface ProviderServiceItem {
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
export class ProviderService {

    private baseUrl = 'http://localhost:5128/api/pvdservices';
    private http = inject(HttpClient);
    getAll(): Observable<ProviderServiceItem[]> {
        return this.http.get<ProviderServiceItem[]>(this.baseUrl);
    }
    getById(id: number): Observable<ProviderServiceItem> {
        return this.http.get<ProviderServiceItem>(`${this.baseUrl}/${id}`);
    }
    create(body: any) {
        return this.http.post(this.baseUrl, body);
    }
    delete(id: number) {
        return this.http.delete(`${this.baseUrl}/${id}`);
    }
}
