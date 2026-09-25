import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, tap, throwError } from 'rxjs';
import { ServerStatus } from '../services/server-status/server-status';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  const serverStatus = inject(ServerStatus);

  const token = localStorage.getItem('token');

  // Nếu có token thì gắn vào Authorization Header
  if (token) {
    req = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
  }

  return next(req).pipe(
    tap(() => serverStatus.markAvailable()),
    catchError((error) => {

      if (error.status === 0) {
        serverStatus.markUnavailable();
      }

      // 401 - Token không hợp lệ / hết hạn / chưa đăng nhập
      if (error.status === 401) {
        localStorage.removeItem('token');
        localStorage.removeItem('user');

        router.navigate(['/login']);
      }

      // 403 - Đã đăng nhập nhưng không có quyền
      if (error.status === 403) {
        alert('Bạn không có quyền truy cập chức năng này.');
      }

      console.error('Auth Interceptor:', error);

      return throwError(() => error);
    })
  );
};