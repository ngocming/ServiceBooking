import { Routes } from '@angular/router';
import { authGuard } from './guards/auth-guard';
import { roleGuard } from './guards/role-guard';

export const routes: Routes = [
  {
    path: 'login',
    loadComponent: () => import('./pages/login/login').then(m => m.Login)
  },
  {
    path: 'register',
    loadComponent: () => import('./pages/register/register').then(m => m.Register)
  },
  {
    path: 'forbidden',
    loadComponent: () => import('./pages/forbidden/forbidden').then(m => m.Forbidden)
  },
  {
    path: 'home',
    loadComponent: () => import('./pages/home/home').then(m => m.Home),
    canActivate: [authGuard]
  },
  {
    path: 'customer/bookings',
    loadComponent: () => import('./pages/bookings/bookings').then(m => m.Bookings),
    canActivate: [authGuard, roleGuard],
    data: { roles: ['Customer'] }
  },
  {
    path: 'provider/bookings',
    loadComponent: () => import('./pages/bookings/bookings').then(m => m.Bookings),
    canActivate: [authGuard, roleGuard],
    data: { roles: ['Provider'] }
  },
  {
    path: 'provider/services/create',
    loadComponent: () => import('./pages/services/create-service/create-service').then(m => m.CreateService),
    canActivate: [authGuard, roleGuard],
    data: { roles: ['Provider'] }
  },
  {
    path: 'provider/services',
    loadComponent: () => import('./pages/services/services').then(m => m.Services),
    canActivate: [authGuard, roleGuard],
    data: { roles: ['Provider'] }
  },
  {
    path: 'customer/services',
    loadComponent: () => import('./pages/services/services').then(m => m.Services),
    canActivate: [authGuard, roleGuard],
    data: { roles: ['Customer'] }
  },
  {
    path: 'services/:id',
    loadComponent: () => import('./pages/services/service-detail/service-detail').then(m => m.ServiceDetail),
    canActivate: [authGuard, roleGuard],
    data: { roles: ['Customer', 'Provider'] }
  },
  { path: '', redirectTo: 'login', pathMatch: 'full' },
];
