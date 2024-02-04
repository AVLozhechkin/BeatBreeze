import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from 'src/app/core/services/auth.service';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);

  if (authService.user()) {
    return true;
  }

  const router: Router = inject(Router);

  return router.createUrlTree(['login']);
};
