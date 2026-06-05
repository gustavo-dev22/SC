import { CommonModule } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { PostulanteCertificacionService } from '../../../services/postulante-certificacion.service';
import Swal from 'sweetalert2';
import { ModalCertificacion } from './modal-certificacion/modal-certificacion';
import { AlertService } from '../../../shared/services/alert.service';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

@Component({
  selector: 'app-certificacion',
  imports: [CommonModule, MatDialogModule, MatButtonModule, MatIconModule, MatTooltipModule, MatProgressSpinnerModule],
  templateUrl: './certificacion.html',
  styleUrl: './certificacion.css',
})
export class Certificacion implements OnInit {
  private dialog = inject(MatDialog);
  private certService = inject(PostulanteCertificacionService);
  private alertService = inject(AlertService);

  public listaCertificaciones = signal<any[]>([]);
  private idPostulante!: number;
  public cargando = signal<boolean>(false);

  ngOnInit(): void {
    const profile = JSON.parse(sessionStorage.getItem('user_profile') || '{}');
    const tokenParts = atob(profile.token).split('-');
    this.idPostulante = Number(tokenParts[1]);
    this.cargarCertificaciones();
  }

  cargarCertificaciones(): void {
    this.cargando.set(true); 
    this.certService.getCertificaciones(this.idPostulante).subscribe({
      next: (res) => {
        if (res.success) this.listaCertificaciones.set(res.data);
        this.cargando.set(false); 
      },
      error: () => this.cargando.set(false)
    });
  }

  abrirModal(elemento: any = null): void {
    const dialogRef = this.dialog.open(ModalCertificacion, {
      panelClass: 'custom-academic-dialog-panel',
      disableClose: true,
      autoFocus: 'first-tabbable',
      data: { elemento }
    });

    dialogRef.afterClosed().subscribe(payload => {
      if (payload) {
        payload.idPostulante = this.idPostulante;
        
        this.cargando.set(true); // 🚀 Enciende durante la actualización en backend (.NET)
        this.certService.mantenimiento(payload).subscribe({
          next: () => {
            this.alertService.exito('¡Éxito!', 'Certificación actualizada correctamente.');
            this.cargarCertificaciones(); // El cargador del listado se encargará de apagar el spinner
          },
          error: () => this.cargando.set(false)
        });
      }
    });
  }

  eliminar(id: number): void {
    this.alertService.confirmacion(
      '¿Retirar Certificación?', 
      'Esta acción eliminará de forma permanente el registro de su capacitación. ¿Desea continuar?'
    ).subscribe(confirmado => {
      if (confirmado) {
        const payload = { accion: 'ELIMINAR', idCertificacion: id, idPostulante: this.idPostulante, idTipoEstudioCat:0, nombreEstudio:'', institucion:'', horasAcademicas:0, fechaEmision: new Date() };
        
        this.cargando.set(true);
        this.certService.mantenimiento(payload).subscribe({
          next: () => {
            this.cargarCertificaciones();
          },
          error: () => this.cargando.set(false)
        });
      }
    });
  }
}
