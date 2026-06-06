import { Component, inject, Input, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { PostulanteIdiomaService } from '../../../services/postulante-idioma.service';
import { AlertService } from '../../../shared/services/alert.service';
import { ModalIdiomaComponent } from './modal-idioma/modal-idioma';

@Component({
  selector: 'app-idiomas',
  standalone: true,
  imports: [CommonModule, MatDialogModule, MatButtonModule, MatIconModule, MatTooltipModule, MatProgressSpinnerModule],
  templateUrl: './idioma.html',
  styleUrls: ['./idioma.css']
})
export class Idiomas implements OnInit {
  @Input() modoLectura = false;
  private dialog = inject(MatDialog);
  private idiomaService = inject(PostulanteIdiomaService);
  private alertService = inject(AlertService);

  public listaIdiomas = signal<any[]>([]);
  public cargando = signal<boolean>(false);
  private idPostulante!: number;

  ngOnInit(): void {
    const profile = JSON.parse(sessionStorage.getItem('user_profile') || '{}');
    const tokenParts = atob(profile.token).split('-');
    this.idPostulante = Number(tokenParts[1]);
    this.cargarIdiomas();
  }

  cargarIdiomas(): void {
    this.cargando.set(true);
    this.idiomaService.getIdiomas(this.idPostulante).subscribe({
      next: (res) => {
        if (res.success) this.listaIdiomas.set(res.data);
        this.cargando.set(false);
      },
      error: () => this.cargando.set(false)
    });
  }

  abrirModal(elemento: any = null): void {
    const dialogRef = this.dialog.open(ModalIdiomaComponent, {
      panelClass: 'custom-academic-dialog-panel',
      disableClose: true,
      autoFocus: 'first-tabbable',
      data: { elemento }
    });

    dialogRef.afterClosed().subscribe(payload => {
      if (payload) {
        payload.idPostulante = this.idPostulante;
        this.cargando.set(true);
        this.idiomaService.mantenimiento(payload).subscribe({
          next: () => {
            this.alertService.exito('¡Éxito!', 'Dominio lingüístico guardado correctamente.');
            this.cargarIdiomas();
          },
          error: () => this.cargando.set(false)
        });
      }
    });
  }

  eliminar(id: number): void {
    this.alertService.confirmacion('¿Retirar Idioma?', 'Esta acción quitará el idioma seleccionado de su currículum.').subscribe(confirmado => {
      if (confirmado) {
        const payload = { accion: 'ELIMINAR', idPostulanteIdioma: id, idPostulante: this.idPostulante, idIdiomaCat:0, idNivelHablaCat:0, idNivelLecturaCat:0, idNivelEscrituraCat:0 };
        this.cargando.set(true);
        this.idiomaService.mantenimiento(payload).subscribe({
          next: () => this.cargarIdiomas(),
          error: () => this.cargando.set(false)
        });
      }
    });
  }
}