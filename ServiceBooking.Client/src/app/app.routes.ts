import { Routes } from '@angular/router';
import { Bookings } from './pages/bookings/bookings';
import { Login } from './pages/login/login';
import { Register } from './pages/register/register';
import { authGuard } from './guards/auth-guard';
export const routes: Routes = [
  { path: 'login', component: Login },
  { path: 'register', component: Register },
  { path: 'bookings', component: Bookings, canActivate: [authGuard] },
  { path: '', redirectTo: 'login', pathMatch: 'full' },
];
