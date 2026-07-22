import { Component, inject, Input, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { PostulanteColegiaturaService } from '../../../services/postulante-colegiatura.service';
import { AlertService } from '../../../shared/services/alert.service';
import { ModalColegiatura } from './modal-colegiatura/modal-colegiatura';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-colegiatura',
  standalone: true,
  imports: [CommonModule, MatDialogModule, MatButtonModule, MatIconModule, MatTooltipModule, MatProgressSpinnerModule],
  templateUrl: './colegiatura.html',
  styleUrls: ['./colegiatura.css']
})
export class Colegiatura implements OnInit {
  @Input() modoLectura = false;
  private dialog = inject(MatDialog);
  private colegiaturaService = inject(PostulanteColegiaturaService);
  private alertService = inject(AlertService);
  private authService = inject(AuthService);

  public listaColegiaturas = signal<any[]>([]);
  public cargando = signal<boolean>(false);
  private idPostulante!: number;

  ngOnInit(): void {   
    this.idPostulante = this.authService.obtenerIdPostulanteDesdeJwt();
    
    if (this.idPostulante > 0) {
      this.cargarColegiaturas();
    } else {
      this.alertService.error('Error de Sesión', 'No se pudo identificar al postulante. Por favor reinicie sesión.');
    }
  }

  cargarColegiaturas(): void {
    this.cargando.set(true);
    this.colegiaturaService.getColegiaturas(this.idPostulante).subscribe({
      next: (res) => {
        if (res.success) this.listaColegiaturas.set(res.data);
        this.cargando.set(false);
      },
      error: () => this.cargando.set(false)
    });
  }

  abrirModal(elemento: any = null): void {
    const dialogRef = this.dialog.open(ModalColegiatura, {
      panelClass: 'custom-academic-dialog-panel',
      disableClose: true,
      autoFocus: 'first-tabbable',
      data: { elemento }
    });

    dialogRef.afterClosed().subscribe(payload => {
      if (payload) {
        payload.idPostulante = this.idPostulante;
        this.cargando.set(true);
        this.colegiaturaService.mantenimiento(payload).subscribe({
          next: () => {
            this.alertService.exito('¡Éxito!', 'Colegiatura registrada correctamente.');
            this.cargarColegiaturas();
          },
          error: () => this.cargando.set(false)
        });
      }
    });
  }

  eliminar(id: number): void {
    this.alertService.confirmacion('¿Retirar Colegiatura?', 'Se borrará el registro de colegiación.').subscribe(confirmado => {
      if (confirmado) {
        const payload = { accion: 'ELIMINAR', idColegiatura: id, idPostulante: this.idPostulante, idColegioCat:0, numeroColegiacion:'', fechaColegiacion: new Date(), certificadoHabilitacionRuta:'' };
        this.cargando.set(true);
        this.colegiaturaService.mantenimiento(payload).subscribe({
          next: () => this.cargarColegiaturas(),
          error: () => this.cargando.set(false)
        });
      }
    });
  }
}