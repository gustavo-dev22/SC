import { CommonModule } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import Swal from 'sweetalert2';
import { ModalFormacion } from './modal-formacion/modal-formacion';
import { PostulanteFormacionService } from '../../../services/postulante-formacion.service';

@Component({
  selector: 'app-formacion',
  imports: [CommonModule, MatDialogModule, MatButtonModule, MatIconModule],
  templateUrl: './formacion.html',
  styleUrl: './formacion.css',
})
export class Formacion implements OnInit{
  private dialog = inject(MatDialog);
  private formacionService = inject(PostulanteFormacionService);
  
  public listaFormacion = signal<any[]>([]);
  private idPostulante!: number;

  ngOnInit(): void {
    const profile = JSON.parse(sessionStorage.getItem('user_profile') || '{}');
    const tokenParts = atob(profile.token).split('-');
    this.idPostulante = Number(tokenParts[1]);
    this.cargarFormacion();
  }

  cargarFormacion(): void {
    this.formacionService.getFormacion(this.idPostulante).subscribe(res => {
      if (res.success) this.listaFormacion.set(res.data);
    });
  }

  abrirModal(elemento: any = null): void {
    const dialogRef = this.dialog.open(ModalFormacion, { width: '560px', maxWidth: '95vw', disableClose: true, panelClass: 'custom-academic-dialog-panel', data: { elemento } });
    dialogRef.afterClosed().subscribe(payload => {
      if (payload) {
        payload.idPostulante = this.idPostulante;
        this.formacionService.mantenimiento(payload).subscribe(() => {
          Swal.fire('¡Éxito!', 'Historial académico actualizado.', 'success');
          this.cargarFormacion();
        });
      }
    });
  }

  eliminar(id: number): void {
    Swal.fire({ title: '¿Eliminar?', text: 'Se borrará el registro.', icon: 'warning', showCancelButton: true }).then(r => {
      if (r.isConfirmed) {
        const payload = { accion: 'ELIMINAR', idFormacion: id, idPostulante: this.idPostulante, idNivelCat:0, idEstadoCat:0, institucion:'', carrera:'', mesInicio:0, anioInicio:0 };
        this.formacionService.mantenimiento(payload).subscribe(() => this.cargarFormacion());
      }
    });
  }
}
