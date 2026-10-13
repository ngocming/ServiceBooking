import { Routes } from '@angular/router';
import { Bookings } from './pages/bookings/bookings';
import { Login } from './pages/login/login';
import { Register } from './pages/register/register';
import { Forbidden } from './pages/forbidden/forbidden';
import { Services } from './pages/services/services';
import { CreateService } from './pages/services/create-service/create-service';
import { authGuard } from './guards/auth-guard';
import { roleGuard } from './guards/role-guard';
import { ServiceDetail } from './pages/services/service-detail/service-detail';
import { Home } from './pages/home/home';

export const routes: Routes = [
  { path: 'login', component: Login },
  { path: 'register', component: Register },
  { path: 'forbidden', component: Forbidden },
  { path: 'home', component: Home, canActivate: [authGuard] },

  {
    path: 'customer/bookings',
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
  {
    path: 'provider/services/create',
    component: CreateService,
    canActivate: [authGuard, roleGuard],
    data: { roles: ['Provider'] }
  },
  {
    path: 'provider/services',
    component: Services,
    canActivate: [authGuard, roleGuard],
    data: { roles: ['Provider'] }
  },
  {
    path: 'customer/services',
    component: Services,
    canActivate: [authGuard, roleGuard],
    data: { roles: ['Customer'] }
  },
  {
    path: 'services/:id',
    component: ServiceDetail,
    canActivate: [authGuard, roleGuard],
    data: { roles: ['Customer', 'Provider'] }
  },
  { path: '', redirectTo: 'login', pathMatch: 'full' },
];
