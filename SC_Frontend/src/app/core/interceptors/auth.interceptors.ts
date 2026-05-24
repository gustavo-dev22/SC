import { HttpInterceptorFn } from '@angular/common/http';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  // Obtenemos el token guardado en el almacenamiento local
  const token = sessionStorage.getItem('token');

  // Clonamos la petición original y le adjuntamos la cabecera Authorization de forma segura
  if (token && token !== 'null') {
    const authReq = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
    return next(authReq);
  }

  // Si no hay token, la petición continúa su flujo normal sin alteración
  return next(req);
};