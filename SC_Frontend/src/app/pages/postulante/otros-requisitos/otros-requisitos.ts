import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { PostulanteOtrosRequisitosService } from '../../../services/postulante-otros-requisitos.service';
import { AlertService } from '../../../shared/services/alert.service';
import { ModalOtrosRequisitos } from './modal-otros-requisitos/modal-otros-requisitos';

@Component({
  selector: 'app-otros-requisitos',
  standalone: true,
  imports: [CommonModule, MatDialogModule, MatButtonModule, MatIconModule, MatTooltipModule, MatProgressSpinnerModule],
  templateUrl: './otros-requisitos.html',
  styleUrls: ['./otros-requisitos.css']
})
export class OtrosRequisitos implements OnInit {
  private dialog = inject(MatDialog);
  private reqService = inject(PostulanteOtrosRequisitosService);
  private alertService = inject(AlertService);

  public listaRequisitos = signal<any[]>([]);
  public cargando = signal<boolean>(false);
  private idPostulante!: number;

  ngOnInit(): void {
    const profile = JSON.parse(sessionStorage.getItem('user_profile') || '{}');
    const tokenParts = atob(profile.token).split('-');
    this.idPostulante = Number(tokenParts[1]);
    this.cargarRequisitos();
  }

  cargarRequisitos(): void {
    this.cargando.set(true);
    this.reqService.getRequisitos(this.idPostulante).subscribe({
      next: (res) => {
        if (res.success) this.listaRequisitos.set(res.data);
        this.cargando.set(false);
      },
      error: () => this.cargando.set(false)
    });
  }

  abrirModal(elemento: any = null): void {
    const dialogRef = this.dialog.open(ModalOtrosRequisitos, {
      panelClass: 'custom-academic-dialog-panel',
      disableClose: true,
      autoFocus: 'first-tabbable',
      data: { elemento }
    });

    dialogRef.afterClosed().subscribe(payload => {
      if (payload) {
        payload.idPostulante = this.idPostulante;
        this.cargando.set(true);
        this.reqService.mantenimiento(payload).subscribe({
          next: () => {
            this.alertService.exito('¡Éxito!', 'Requisito especial actualizado con éxito.');
            this.cargarRequisitos();
          },
          error: () => this.cargando.set(false)
        });
      }
    });
  }

  eliminar(id: number): void {
    this.alertService.confirmacion('¿Eliminar Requisito?', 'Esta acción retirará de forma permanente esta acreditación.').subscribe(confirmado => {
      if (confirmado) {
        const payload = { accion: 'ELIMINAR', idRequisitoEspecial: id, idPostulante: this.idPostulante, idTipoRequisitoCat: 0, descripcionDocumento:'', numeroRegistro:'', fechaEmision: null, fechaVencimiento: null };
        this.cargando.set(true);
        this.reqService.mantenimiento(payload).subscribe({
          next: () => this.cargarRequisitos(),
          error: () => this.cargando.set(false)
        });
      }
    });
  }
}
