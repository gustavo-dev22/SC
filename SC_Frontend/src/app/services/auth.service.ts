import { inject, Injectable } from '@angular/core';
import { CrudHttpService } from '../core/services/crud-http.service';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private crudHttp = inject(CrudHttpService);

  login(usuario: string, contrasena: string, esExterno: boolean): Observable<any> {
    const payload = {
      username: usuario,
      password: contrasena,
      isExternal: esExterno
    };

    return this.crudHttp.post<any>('Auth/login', payload);
  }

  registrarPostulante(payload: any): Observable<any> {
    return this.crudHttp.post<any>('public/auth/registro', payload);
  }

  solicitarEnlaceRecuperacion(numDocumento: string): Observable<any> {
    return this.crudHttp.post('public/auth/solicitar-recuperacion', { numDocumento });
  }

  confirmarRestablecimiento(token: string, nuevoPassword: string): Observable<any> {
    return this.crudHttp.post('public/auth/restablecer-password', { token, nuevoPassword });
  }
}