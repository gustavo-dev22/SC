import { Component, inject, OnInit, signal } from '@angular/core';
import { ComiteEvaluacionService } from '../../../services/comite-evaluacion.service';
import { AlertService } from '../../../shared/services/alert.service';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { FormsModule } from '@angular/forms';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatCardModule } from '@angular/material/card';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { ModalDeclararDesierta } from './modal-declarar-desierta/modal-declarar-desierta';
import { MatDialog } from '@angular/material/dialog';

@Component({
  selector: 'app-cuadro-merito',
  imports: [CommonModule, MatTableModule, MatPaginatorModule, MatFormFieldModule, MatInputModule, FormsModule, MatProgressSpinnerModule, MatCardModule, MatSelectModule, MatButtonModule, MatIconModule],
  templateUrl: './cuadro-merito.html',
  styleUrl: './cuadro-merito.css',
})
export class CuadroMerito implements OnInit{
  private comiteService = inject(ComiteEvaluacionService);
  private alertService = inject(AlertService);
  private dialog = inject(MatDialog);

  public plazas = signal<any[]>([]);
  public plazaSeleccionada = signal<number | null>(null);
  public listaPostulantes = signal<any[]>([]);
  public cargandoPlazas = signal<boolean>(false);
  public cargando = signal<boolean>(false);

  ngOnInit(): void {
    this.cargarPlazasVigentes();
  }

  cargarPlazasVigentes(): void {
    this.cargandoPlazas.set(true);
    this.comiteService.listarPlazasAsignadasComite().subscribe({
      next: (res) => {
        if (res && res.content) this.plazas.set(res.content);
        this.cargandoPlazas.set(false);
      },
      error: () => this.cargandoPlazas.set(false)
    });
  }

  onPlazaChange(idPlaza: number): void {
    this.plazaSeleccionada.set(idPlaza);
    this.cargarReporteFinal(idPlaza);
  }

  cargarReporteFinal(idPlaza: number): void {
    this.cargando.set(true);
    this.comiteService.obtenerCuadroMeritoFinal(idPlaza).subscribe({
      next: (res) => {
        if (res.success) {
          this.listaPostulantes.set(res.data);
        }
        this.cargando.set(false);
      },
      error: () => this.cargando.set(false)
    });
  }

  exportarReportePdf(): void {
    const idPlaza = this.plazaSeleccionada();
    if (!idPlaza) return;

    const plazaActual = this.plazas().find(p => p.idPlaza === idPlaza);
    const nombrePuesto = plazaActual ? plazaActual.nombrePuesto : idPlaza.toString();
    const nombrePuestoFormateado = nombrePuesto.toUpperCase().replace(/ /g, '_');

    this.cargando.set(true);
    
    this.comiteService.descargarActaFinalPdf(idPlaza).subscribe({
      next: (blobData: Blob) => {
        const urlTemporal = window.URL.createObjectURL(blobData);
        const enlaceDescarga = document.createElement('a');
        enlaceDescarga.href = urlTemporal;
        enlaceDescarga.download = `Acta_Resultados_Consolidados_${nombrePuestoFormateado}.pdf`;
        
        document.body.appendChild(enlaceDescarga);
        enlaceDescarga.click();
        document.body.removeChild(enlaceDescarga);
        window.URL.revokeObjectURL(urlTemporal);
        
        this.cargando.set(false);
      },
      error: () => {
        this.cargando.set(false);
        this.alertService.error('Error', 'No se pudo generar el acta consolidada final.');
      }
    });
  }

  abrirModalDesierta(): void {
    const idPlaza = this.plazaSeleccionada();
    if (!idPlaza) return;

    const plazaActual = this.plazas().find(p => p.idPlaza === idPlaza);

    const dialogRef = this.dialog.open(ModalDeclararDesierta, {
      width: '520px',
      disableClose: true,
      data: { 
        idPlaza, 
        codigoConvocatoria: plazaActual ? plazaActual.codigoConvocatoria : idPlaza.toString() 
      }
    });

    dialogRef.afterClosed().subscribe(formValue => {
      if (formValue) {
        this.alertService.confirmacion(
          '¿Declarar Plaza Desierta?', 
          'Esta acción registrará el acta de deserción y dará por concluido el proceso evaluativo de esta plaza.'
        ).subscribe(confirmado => {
          if (confirmado) {
            const payload = {
              idPlaza,
              ...formValue
            };

            this.cargando.set(true);
            this.comiteService.declararPlazaDesierta(payload).subscribe({
              next: (res) => {
                if (res.success) {
                  this.alertService.exito('Proceso Concluido', 'La plaza ha sido declarada desierta correctamente.');
                  this.cargarReporteFinal(idPlaza); // Refresca el cuadro de méritos
                  this.exportarActaDesiertaPdf(idPlaza);
                }
                this.cargando.set(false);
              },
              error: () => this.cargando.set(false)
            });
          }
        });
      }
    });
  }

  exportarActaDesiertaPdf(idPlaza: number): void {
    this.cargando.set(true);
    this.comiteService.descargarActaDesiertaPdf(idPlaza).subscribe({
      next: (blobData: Blob) => {
        const urlTemporal = window.URL.createObjectURL(blobData);
        const enlaceDescarga = document.createElement('a');
        enlaceDescarga.href = urlTemporal;
        enlaceDescarga.download = `Acta_Declaracion_Desierta_Plaza_${idPlaza}.pdf`;
        
        document.body.appendChild(enlaceDescarga);
        enlaceDescarga.click();
        document.body.removeChild(enlaceDescarga);
        window.URL.revokeObjectURL(urlTemporal);
        
        this.cargando.set(false);
      },
      error: () => {
        this.cargando.set(false);
        this.alertService.error('Error', 'No se pudo generar el acta de deserción.');
      }
    });
  }
}
