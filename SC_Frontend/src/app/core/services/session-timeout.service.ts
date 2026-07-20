import { inject, Injectable, Injector, NgZone, signal } from '@angular/core';
import { Router } from '@angular/router';
import { fromEvent, merge, Subscription, timer } from 'rxjs';
import { switchMap } from 'rxjs/operators';
import Swal from 'sweetalert2';
import { AuthService } from '../../services/auth.service';

@Injectable({
  providedIn: 'root'
})
export class SessionTimeoutService {
  private router = inject(Router);
  private injector = inject(Injector);
  private ngZone = inject(NgZone);

  // 🚀 TIEMPO LÍMITE: 5 minutos (5 * 60 * 1000 ms = 300000 ms)
  private readonly TIMEOUT_MS = 5 * 60 * 1000; 

  private activitySubscription?: Subscription;
  private tabHiddenTimer?: any;

  /**
   * Inicia los escuchadores de inactividad física y cambio de pestaña
   */
  public iniciarMonitoreo(): void {
    this.detenerMonitoreo(); // Limpia suscripciones previas por seguridad

    // 1. MONITOREO DE INACTIVIDAD FÍSICA (Mouse, Teclado, Scroll)
    // Ejecutamos fuera de Angular Zone para evitar disparar la detección de cambios en cada movimiento del mouse
    this.ngZone.runOutsideAngular(() => {
      const userActivityEvents$ = merge(
        fromEvent(window, 'mousemove'),
        fromEvent(window, 'click'),
        fromEvent(window, 'keydown'),
        fromEvent(window, 'scroll'),
        fromEvent(window, 'touchstart')
      );

      this.activitySubscription = userActivityEvents$
        .pipe(
          // Cada vez que detecta actividad, reinicia un temporizador de 5 minutos
          switchMap(() => timer(this.TIMEOUT_MS))
        )
        .subscribe(() => {
          // Al cumplirse los 5 minutos sin actividad, regresamos a Angular Zone para cerrar sesión
          this.ngZone.run(() => {
            this.cerrarSesionPorInactividad('Ha excedido el tiempo de inactividad de 5 minutos.');
          });
        });
    });

    // 2. MONITOREO DE CAMBIO DE PESTAÑA (Pestaña oculta en segundo plano)
    this.escucharCambioPestaña();
  }

  /**
   * Detiene los temporizadores y libera memoria
   */
  public detenerMonitoreo(): void {
    if (this.activitySubscription) {
      this.activitySubscription.unsubscribe();
    }
    if (this.tabHiddenTimer) {
      clearTimeout(this.tabHiddenTimer);
    }
  }

  /**
   * Escucha si el usuario cambia de pestaña en el navegador
   */
  private escucharCambioPestaña(): void {
    document.addEventListener('visibilitychange', () => {
      if (document.hidden) {
        // 🚀 Si oculta la pestaña o cambia de ventana, arranca el conteo de 5 minutos
        this.tabHiddenTimer = setTimeout(() => {
          this.cerrarSesionPorInactividad('Su sesión ha caducado por mantener la pestaña en segundo plano.');
        }, this.TIMEOUT_MS);
      } else {
        // 🚀 Si el usuario regresa a la pestaña antes de los 5 minutos, cancelamos el temporizador
        if (this.tabHiddenTimer) {
          clearTimeout(this.tabHiddenTimer);
        }
      }
    });
  }

  /**
   * Cierra la sesión, limpia storage y muestra la alerta informativa
   */
  private cerrarSesionPorInactividad(mensajeExplicativo: string): void {
    this.detenerMonitoreo();

    // 🚀 RESOLUCIÓN LAZY DE AUTHERVICE: Rompe la dependencia circular en caliente
    const authService = this.injector.get(AuthService);

    if (authService && typeof authService.logout === 'function') {
      authService.logout();
    } else {
      sessionStorage.clear();
      localStorage.clear();
    }

    // 2. Alerta SweetAlert2
    Swal.fire({
      title: 'Sesión Expirada',
      text: mensajeExplicativo,
      icon: 'warning',
      confirmButtonText: 'Entendido / Ir al Login',
      confirmButtonColor: '#1e3c72',
      allowOutsideClick: false,
      allowEscapeKey: false,
      heightAuto: false
    }).then(() => {
      // 3. Redirección forzada al Login
      this.router.navigate(['/login']);
    });
  }
}