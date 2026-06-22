import { Component, OnInit, inject, signal } from '@angular/core';
import { NotificacionesService } from '../../../services/notificaciones.service';
import { CommonModule } from '@angular/common';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-alertas-notificaciones',
  imports: [CommonModule, MatIconModule, MatProgressSpinnerModule],
  templateUrl: './alertas-notificaciones.html',
  styleUrl: './alertas-notificaciones.css',
})
export class AlertasNotificaciones implements OnInit {
  private ns = inject(NotificacionesService);

  public listaNotificaciones = signal<any[]>([]);
  public cargando = signal<boolean>(false);
  private idPostulante!: number;

  ngOnInit(): void {
    const profile = JSON.parse(sessionStorage.getItem('user_profile') || '{}');
    const tokenParts = atob(profile.token).split('-');
    this.idPostulante = Number(tokenParts[1]);

    this.cargarNotificaciones();
  }

  cargarNotificaciones(): void {
    this.cargando.set(true);
    this.ns.listarNotificaciones(this.idPostulante).subscribe({
      next: (res) => {
        if (res.success) {
          this.listaNotificaciones.set(res.data);
          //this.marcarTodasComoLeidasAlEntrar(res.data);
        }
        this.cargando.set(false);
      },
      error: () => this.cargando.set(false)
    });
  }

  marcarIndividualComoLeida(notif: any): void {
    if (notif.leido) return; // Si ya está leída, no hacemos nada

    this.ns.marcarComoLeida(notif.idNotificacion).subscribe({
      next: (res) => {
        if (res.success) {
          // Mutación reactiva de la Signal en memoria pura
          this.listaNotificaciones.update(notificaciones =>
            notificaciones.map(n => n.idNotificacion === notif.idNotificacion ? { ...n, leido: true } : n)
          );
        }
      }
    });
  }

  private marcarTodasComoLeidasAlEntrar(notificaciones: any[]): void {
    notificaciones.forEach(n => {
      if (!n.leido) {
        this.ns.marcarComoLeida(n.idNotificacion).subscribe({
          next: () => {
            this.listaNotificaciones.update(list => 
              list.map(not => not.idNotificacion === n.idNotificacion ? { ...not, leido: true } : not)
            );
          }
        });
      }
    });
  }
}