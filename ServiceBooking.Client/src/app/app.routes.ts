import { Routes } from '@angular/router';
import { Bookings } from './pages/bookings/bookings';
import { Login } from './pages/login/login';
import { Register } from './pages/register/register';
import { Forbidden } from './pages/forbidden/forbidden';
import { authGuard } from './guards/auth-guard';
import { roleGuard } from './guards/role-guard';

export const routes: Routes = [
  { path: 'login', component: Login },
  { path: 'register', component: Register },
  { path: 'forbidden', component: Forbidden },
  {
    path: 'bookings',
    component: Bookings,
    canActivate: [authGuard, roleGuard],
    data: { roles: ['Customer'] }
  },
  {
    path: 'provider/bookings',
    component: Bookings,
    canActivate: [authGuard, roleGuard],
    data: { roles: ['Provider'] }
  },
  { path: '', redirectTo: 'login', pathMatch: 'full' },
];
