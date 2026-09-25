import { Inject, Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface ProviderService {
    id: number;
    name: string;
    description?: string;
    price: number;
    durationMinutes: number;
    providerId: number;
}

export interface CreateProviderServiceDTO {
    name: string;
    description?: string;
    price: number;
    durationMinutes: number;
    providerId: number;
}

@Injectable()
export class ProviderServices {
    private baseUrl = 'http://localhost:5128/api/providder';
    constructor(@Inject(HttpClient) private http: HttpClient) { }
    
    getAllProviderServices(): Observable<ProviderService[]> {
        return this.http.get<ProviderService[]>(this.baseUrl);
    }
    
    getProviderServiceById(id: number): Observable<ProviderService> {
        return this.http.get<ProviderService>(`${this.baseUrl}/${id}`);
    }
    
    createProviderService(serviceData: CreateProviderServiceDTO): Observable<ProviderService> {
        const headers = new HttpHeaders({
            'Content-Type': 'application/json'
        });
        return this.http.post<ProviderService>(this.baseUrl, serviceData, { headers });
    }
    
    deleteProviderService(id: number): Observable<void> {
        return this.http.delete<void>(`${this.baseUrl}/${id}`);
    }
}
