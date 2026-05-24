import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';

export const authGuard: CanActivateFn = (route, state) => {
  const router = inject(Router);
  const token = sessionStorage.getItem('token');

  if (token) {
    // El token existe, el acceso está permitido
    return true;
  }

  // No hay token activo, redirigimos de inmediato a la pantalla de Login
  router.navigate(['/login']);
  return false;
};