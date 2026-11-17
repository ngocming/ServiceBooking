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
    loadComponent: () => import('./pages/home/home').then(m => m.Home)
  },
  {
    path: 'services',
    loadComponent: () => import('./pages/services/services').then(m => m.Services)
  },
  {
    path: 'services/:id',
    loadComponent: () => import('./pages/services/service-detail/service-detail').then(m => m.ServiceDetail)
  },
  {
    path: 'provider/dashboard',
    loadComponent: () => import('./pages/provider-dashboard/provider-dashboard').then(m => m.ProviderDashboard),
    canActivate: [authGuard, roleGuard],
    data: { roles: ['Provider'] }
  },
  {
    path: 'provider/profile',
    loadComponent: () => import('./pages/provider-profile/provider-profile').then(m => m.ProviderProfilePage),
    canActivate: [authGuard, roleGuard],
    data: { roles: ['Provider'] }
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
    loadComponent: () => import('./pages/services/services').then(m => m.Services)
  },
  { path: '', redirectTo: 'home', pathMatch: 'full' },
];
