import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from '../../services/auth.service';

export const authGuard: CanActivateFn = () => {
  const authService = inject(AuthService);
  const router      = inject(Router);

  // ✅ El guard consulta al servicio, no al sessionStorage directamente
  if (authService.isAuthenticated()) return true;

  router.navigate(['/login']);
  return false;
};