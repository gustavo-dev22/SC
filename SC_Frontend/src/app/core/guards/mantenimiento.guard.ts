import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { ParametroService } from '../../services/parametro.service';
import { map, catchError } from 'rxjs/operators';
import { of } from 'rxjs';

export const mantenimientoGuard: CanActivateFn = (route, state) => {
  const parametroService = inject(ParametroService);
  const router = inject(Router);

  const profileRaw = sessionStorage.getItem('user_profile') || '{}';
  
  let esPostulante = false;

  try {
    const profile = JSON.parse(profileRaw);
    esPostulante = profile.rol === 'POSTULANTE' || profile.rol === 'Postulante';
  } catch {
    esPostulante = false;
  }

  if (!esPostulante) {
    return true;
  }

  return parametroService.verificarMantenimientoPortal().pipe(
    map(res => {
      if (res.success && res.enMantenimiento) {
        router.navigate(['/mantenimiento']);
        return false;
      }
      return true;
    }),
    catchError(() => {
      return of(true);
    })
  );
};