import { Service, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';


export interface ProviderProfile {
    id: number;
    userId: number;
    displayName: string;
    description: string;
    phone: string;
    address: string;
    latitude: number;
    longitude: number;
    isAvailable: boolean;
    createdAt: string;
}
@Service()
export class Provider {

    private baseUrl = 'http://localhost:5128/api/providers';
    private http = inject(HttpClient);
    
    getMyProfile(): Observable<ProviderProfile> {
        return this.http.get<ProviderProfile>(
            `${this.baseUrl}/profile`
        );
    }
}
