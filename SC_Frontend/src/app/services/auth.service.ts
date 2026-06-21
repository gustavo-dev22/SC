import { computed, inject, Injectable, signal } from '@angular/core';
import { CrudHttpService } from '../core/services/crud-http.service';
import { catchError, Observable, tap, throwError } from 'rxjs';
import { Router } from '@angular/router';
import { ApiResponse, AuthResponse, LoginRequest, RegistroPostulanteRequest } from '../core/models/auth.model';
import { AUTH_ENDPOINTS } from '../core/constants/api-endpoints';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private crudHttp = inject(CrudHttpService);
  private router   = inject(Router);

  private readonly _usuarioActual = signal<AuthResponse['data'] | null>(
    this.recuperarSesionGuardada()
  );

  public readonly usuarioActual = this._usuarioActual.asReadonly();
  public readonly isAuthenticated = computed(() => !!this._usuarioActual());

  login(request: LoginRequest): Observable<ApiResponse<AuthResponse['data']>> {
    return this.crudHttp
      .post<ApiResponse<AuthResponse['data']>>(AUTH_ENDPOINTS.LOGIN, request)
      .pipe(
        tap(res => {
          if (res.success && res.data) {
            this.guardarSesion(res.data);  // ← responsabilidad centralizada
          }
        }),
        catchError(err => this.manejarError(err))
      );
  }

  registrarPostulante(
    request: RegistroPostulanteRequest
  ): Observable<ApiResponse> {
    return this.crudHttp
      .post<ApiResponse>(AUTH_ENDPOINTS.REGISTRO, request)
      .pipe(catchError(err => this.manejarError(err)));
  }

  solicitarEnlaceRecuperacion(
    numDocumento: string
  ): Observable<ApiResponse> {
    return this.crudHttp
      .post<ApiResponse>(AUTH_ENDPOINTS.SOLICITAR_RECUPERACION, { numDocumento })
      .pipe(catchError(err => this.manejarError(err)));
  }

  confirmarRestablecimiento(
    token: string,
    nuevoPassword: string
  ): Observable<ApiResponse> {
    return this.crudHttp
      .post<ApiResponse>(AUTH_ENDPOINTS.RESTABLECER_PASSWORD, { token, nuevoPassword })
      .pipe(catchError(err => this.manejarError(err)));
  }

  logout(): void {
    sessionStorage.removeItem('token');
    sessionStorage.removeItem('user_profile');
    this._usuarioActual.set(null);
    this.router.navigate(['/login']);
  }

  getToken(): string | null {
    return sessionStorage.getItem('token');
  }

  private guardarSesion(data: AuthResponse['data']): void {
    sessionStorage.setItem('token', data.token);
    sessionStorage.setItem('user_profile', JSON.stringify(data));
    this._usuarioActual.set(data);
  }

  private recuperarSesionGuardada(): AuthResponse['data'] | null {
    try {
      const raw = sessionStorage.getItem('user_profile');
      return raw ? JSON.parse(raw) : null;
    } catch {
      return null;  // JSON corrupto → sesión inválida
    }
  }

  private manejarError(err: any): Observable<never> {
    // Aquí puedes loguear a un servicio de monitoreo (Sentry, etc.)
    return throwError(() => err);
  }
}