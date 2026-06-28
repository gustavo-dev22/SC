import { Component, inject, OnInit, signal } from '@angular/core';
import { TrazabilidadService } from '../../../services/trazabilidad.service';
import { AlertService } from '../../../shared/services/alert.service';
import { CommonModule } from '@angular/common';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-trazabilidad-postulaciones',
  imports: [CommonModule, MatProgressSpinnerModule, MatCardModule, MatFormFieldModule, MatInputModule, FormsModule, MatButtonModule, MatIconModule],
  templateUrl: './trazabilidad-postulaciones.html',
  styleUrl: './trazabilidad-postulaciones.css',
})
export class TrazabilidadPostulaciones implements OnInit {
  private ts = inject(TrazabilidadService);
  private alertService = inject(AlertService);

  private dialogRef = inject(MatDialogRef<TrazabilidadPostulaciones>, { optional: true });
  private dialogData = inject(MAT_DIALOG_DATA, { optional: true });

  public timeline = signal<any[]>([]);
  public cargando = signal<boolean>(false);
  public codigoExpedienteBusqueda = '';

  public esModoModal = signal<boolean>(false);

  ngOnInit(): void {
    if (this.dialogData && this.dialogData.codigoPostulacion) {
      this.esModoModal.set(true);
      this.codigoExpedienteBusqueda = this.dialogData.codigoPostulacion;
      this.buscarExpediente();
    }
  }

  buscarExpediente(): void {
    if (!this.codigoExpedienteBusqueda.trim()) {
      this.alertService.error('Campo Vacío', 'Debe ingresar un número de expediente.');
      return;
    }

    this.cargando.set(true);
    this.ts.consultarTrazabilidad(this.codigoExpedienteBusqueda.trim()).subscribe({
      next: (res) => {
        if (res.success) {
          this.timeline.set(res.data);
        }
        this.cargando.set(false);
      },
      error: () => this.cargando.set(false)
    });
  }

  cerrarModal(): void {
    if (this.dialogRef) this.dialogRef.close();
  }
}
