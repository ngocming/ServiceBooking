import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';

export const roleGuard: CanActivateFn = (route, state) => {
    const router = inject(Router);
    const user = localStorage.getItem('user');

    if (!user) {
        return router.createUrlTree(['/login']);
    }
    try {
        const role = JSON.parse(user).role;
        const allowedRoles = route.data?.['roles'] as string[];
        if (!allowedRoles || allowedRoles.length === 0) {
            return true; 
        }
        if (allowedRoles.includes(role)) {
            return true;
        }
        return router.createUrlTree(['/forbidden']);
    } catch (error) {
        localStorage.removeItem('token');
        localStorage.removeItem('user');
        return router.createUrlTree(['/login']);
    }
    
};
