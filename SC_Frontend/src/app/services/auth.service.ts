import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private http = inject(HttpClient);
  private baseUrl = environment.apiUrl;

  login(usuario: string, contrasena: string, esExterno: boolean): Observable<any> {
    const payload = {
      username: usuario,
      password: contrasena,
      isExternal: esExterno
    };
    // Petición directa a nuestro backend unificado
    return this.http.post<any>(`${this.baseUrl}/Auth/login`, payload);
  }
}
