import { CommonModule } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import Swal from 'sweetalert2';
import { ModalFormacion } from './modal-formacion/modal-formacion';
import { PostulanteFormacionService } from '../../../services/postulante-formacion.service';
import { MatTooltipModule } from '@angular/material/tooltip';
import { AlertService } from '../../../shared/services/alert.service';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

@Component({
  selector: 'app-formacion',
  imports: [CommonModule, MatDialogModule, MatButtonModule, MatIconModule, MatTooltipModule, MatProgressSpinnerModule],
  templateUrl: './formacion.html',
  styleUrl: './formacion.css',
})
export class Formacion implements OnInit{
  private dialog = inject(MatDialog);
  private formacionService = inject(PostulanteFormacionService);
  private alertService = inject(AlertService);
  public cargando = signal(false);
  public listaFormacion = signal<any[]>([]);
  private idPostulante!: number;

  ngOnInit(): void {
    const profile = JSON.parse(sessionStorage.getItem('user_profile') || '{}');
    const tokenParts = atob(profile.token).split('-');
    this.idPostulante = Number(tokenParts[1]);
    this.cargarFormacion();
  }

  cargarFormacion(): void {
    this.cargando.set(true); 
    this.formacionService.getFormacion(this.idPostulante).subscribe({
      next: (res) => {
        if (res.success) this.listaFormacion.set(res.data);
        this.cargando.set(false); 
      },
      error: () => this.cargando.set(false)
    });
  }

  abrirModal(elemento: any = null): void {
    const dialogRef = this.dialog.open(ModalFormacion, { 
      width: '560px', 
      maxWidth: '95vw', 
      disableClose: true, 
      panelClass: 'custom-academic-dialog-panel', 
      data: { elemento } 
    });

    dialogRef.afterClosed().subscribe(payload => {
      if (payload) {
        payload.idPostulante = this.idPostulante;
        
        this.cargando.set(true); 
        this.formacionService.mantenimiento(payload).subscribe({
          next: () => {
            this.alertService.exito('¡Éxito!', 'Historial académico actualizado con éxito.');
            this.cargarFormacion(); 
          },
          error: () => this.cargando.set(false)
        });
      }
    });
  }

  eliminar(id: number): void {
    this.alertService.confirmacion('¿Eliminar Registro?', 'Esta acción retirará de forma permanente este estudio de su currículum. ¿Desea continuar?').subscribe(confirmado => {
      if (confirmado) {
        const payload = { accion: 'ELIMINAR', idFormacion: id, idPostulante: this.idPostulante, idNivelCat: 0, idEstadoCat: 0, institucion: '', carrera: '', mesInicio: 0, anioInicio: 0 };
        
        this.cargando.set(true); 
        this.formacionService.mantenimiento(payload).subscribe({
          next: () => {
            this.cargarFormacion();
          },
          error: () => this.cargando.set(false)
        });
      }
    });
  }
}
