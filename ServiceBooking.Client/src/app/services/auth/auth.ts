import { HttpClient } from '@angular/common/http';
import { inject, Service } from '@angular/core';
import { Observable } from 'rxjs';

export interface LoginResponse {
    token: string;
}

@Service()
export class Auth {
    private baseUrl = 'http://localhost:5128/api/auth';
    private http = inject(HttpClient);
    login(request: any): Observable<LoginResponse> {
        return this.http.post<LoginResponse>(`${this.baseUrl}/login`, request);
    }
    register(request: any): Observable<LoginResponse> {
        return this.http.post<LoginResponse>(`${this.baseUrl}/register`, request);
    }
}
