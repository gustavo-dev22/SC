import { CommonModule } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { DashboardAdminService } from '../../services/dashboard-admin.service';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { PostulanteDashboardService } from '../../services/dashboard-postulante.service';
import { Router } from '@angular/router';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { TrazabilidadPostulaciones } from '../admin/trazabilidad-postulaciones/trazabilidad-postulaciones';
import { PostulanteResumenService } from '../../services/postulante-resumen.service';

@Component({
  selector: 'app-dashboard',
  imports: [CommonModule, MatCardModule, MatProgressSpinnerModule, MatIconModule, MatButtonModule, MatDialogModule],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css',
})
export class Dashboard implements OnInit {
  private dashboardService = inject(DashboardAdminService);
  private postulanteService = inject(PostulanteDashboardService);
  private _postulacionService = inject(PostulanteResumenService);
  private router = inject(Router);
  private dialog = inject(MatDialog);

  // Signal para almacenar la estructura exacta que retorna el Backend
  public dataDashboard = signal<any>(null);
  public dataPostulante = signal<any[]>([]);

  public userRol = signal<string>(''); // 'ADMIN', 'POSTULANTE', 'COMITE'
  public nombreUsuario = signal<string>('');
  public cargando = signal<boolean>(false);

  ngOnInit(): void {
    const profile = JSON.parse(sessionStorage.getItem('user_profile') || '{}');
    this.nombreUsuario.set(profile.nombreCompleto || 'Usuario');
    this.userRol.set(profile.rol || 'POSTULANTE');
    console.log(this.userRol());
    
    if (this.userRol() === 'Administrador') {
      this.cargarDatosDashboard();
    } else if (this.userRol() === 'POSTULANTE') {
      this._postulacionService.consultarEstadoPostulacion().subscribe();
      const tokenParts = atob(profile.token).split('-');
      const idPostulante = Number(tokenParts[1]);
      this.cargarDashboardPostulante(idPostulante);
    }
  }

  cargarDatosDashboard(): void {
    this.cargando.set(true);
    this.dashboardService.obtenerResumen().subscribe({
      next: (res) => {
        if (res.success) {
          this.dataDashboard.set(res.data);
        }
        this.cargando.set(false);
      },
      error: () => {
        this.cargando.set(false);
      }
    });
  }

  cargarDashboardPostulante(idPostulante: number): void {
    this.cargando.set(true);
    this.postulanteService.obtenerResumen(idPostulante).subscribe({
      next: (res) => {
        if (res.success) this.dataPostulante.set(res.data);
        this.cargando.set(false);
      },
      error: () => this.cargando.set(false)
    });
  }

  irAConvocatoriasVigentes(): void {
    this.router.navigate(['/postulante/buscar-plazas']); 
  }

  irADetalleExpediente(codigoPostulacion: string): void {
    this.dialog.open(TrazabilidadPostulaciones, {
      width: '650px',
      maxWidth: '90vw',
      disableClose: false, 
      data: { codigoPostulacion: codigoPostulacion }
    });
  }
}
