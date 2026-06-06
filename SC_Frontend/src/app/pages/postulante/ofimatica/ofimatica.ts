import { Component, inject, Input, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { PostulanteOfimaticaService } from '../../../services/postulante-ofimatica.service';
import { AlertService } from '../../../shared/services/alert.service';
import { ModalOfimatica } from './modal-ofimatica/modal-ofimatica';

@Component({
  selector: 'app-ofimatica',
  standalone: true,
  imports: [CommonModule, MatDialogModule, MatButtonModule, MatIconModule, MatTooltipModule, MatProgressSpinnerModule],
  templateUrl: './ofimatica.html',
  styleUrls: ['./ofimatica.css']
})
export class Ofimatica implements OnInit {
  @Input() modoLectura = false;
  private dialog = inject(MatDialog);
  private ofimaticaService = inject(PostulanteOfimaticaService);
  private alertService = inject(AlertService);

  public listaOfimatica = signal<any[]>([]);
  public cargando = signal<boolean>(false);
  private idPostulante!: number;

  ngOnInit(): void {
    const profile = JSON.parse(sessionStorage.getItem('user_profile') || '{}');
    const tokenParts = atob(profile.token).split('-');
    this.idPostulante = Number(tokenParts[1]);
    this.cargarOfimatica();
  }

  cargarOfimatica(): void {
    this.cargando.set(true);
    this.ofimaticaService.getOfimatica(this.idPostulante).subscribe({
      next: (res) => {
        if (res.success) this.listaOfimatica.set(res.data);
        this.cargando.set(false);
      },
      error: () => this.cargando.set(false)
    });
  }

  abrirModal(elemento: any = null): void {
    const dialogRef = this.dialog.open(ModalOfimatica, {
      panelClass: 'custom-academic-dialog-panel',
      disableClose: true,
      autoFocus: 'first-tabbable',
      data: { elemento }
    });

    dialogRef.afterClosed().subscribe(payload => {
      if (payload) {
        payload.idPostulante = this.idPostulante;
        this.cargando.set(true);
        this.ofimaticaService.mantenimiento(payload).subscribe({
          next: () => {
            this.alertService.exito('¡Éxito!', 'Nivel de ofimática actualizado correctamente.');
            this.cargarOfimatica();
          },
          error: () => this.cargando.set(false)
        });
      }
    });
  }

  eliminar(id: number): void {
    this.alertService.confirmacion('¿Retirar Conocimiento?', 'Se eliminará esta herramienta de su declaración de ofimática.').subscribe(confirmado => {
      if (confirmado) {
        const payload = { accion: 'ELIMINAR', idPostulanteOfimatica: id, idPostulante: this.idPostulante, idHerramientaCat: 0, idNivelCat: 0 };
        this.cargando.set(true);
        this.ofimaticaService.mantenimiento(payload).subscribe({
          next: () => this.cargarOfimatica(),
          error: () => this.cargando.set(false)
        });
      }
    });
  }
}
