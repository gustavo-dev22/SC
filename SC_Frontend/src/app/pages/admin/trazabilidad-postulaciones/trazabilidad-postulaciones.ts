import { Component, inject, signal } from '@angular/core';
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

@Component({
  selector: 'app-trazabilidad-postulaciones',
  imports: [CommonModule, MatProgressSpinnerModule, MatCardModule, MatFormFieldModule, MatInputModule, FormsModule, MatButtonModule, MatIconModule],
  templateUrl: './trazabilidad-postulaciones.html',
  styleUrl: './trazabilidad-postulaciones.css',
})
export class TrazabilidadPostulaciones {
  private ts = inject(TrazabilidadService);
  private alertService = inject(AlertService);

  public timeline = signal<any[]>([]);
  public cargando = signal<boolean>(false);
  public codigoExpedienteBusqueda = '';

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
}
