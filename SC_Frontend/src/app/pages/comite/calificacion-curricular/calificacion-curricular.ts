import { Component, computed, inject, OnInit, signal, viewChild } from '@angular/core';
import { ComiteEvaluacionService } from '../../../services/comite-evaluacion.service';
import { AlertService } from '../../../shared/services/alert.service';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { CommonModule } from '@angular/common';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatSelectModule } from '@angular/material/select';
import { MatIconModule } from '@angular/material/icon';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-calificacion-curricular',
  imports: [CommonModule, MatProgressSpinnerModule, MatTableModule, MatPaginatorModule, MatFormFieldModule, MatInputModule, MatButtonModule, MatCardModule, MatSelectModule, MatIconModule, FormsModule],
  templateUrl: './calificacion-curricular.html',
  styleUrl: './calificacion-curricular.css',
})
export class CalificacionCurricular implements OnInit{
  private comiteService = inject(ComiteEvaluacionService);
  private alertService = inject(AlertService);

  private paginator = viewChild(MatPaginator);

  public plazas = signal<any[]>([]);
  public plazaSeleccionada = signal<number | null>(null);
  public filtroTexto = signal<string>('');
  public cargandoPlazas = signal<boolean>(false);
  public cargando = signal<boolean>(false);
  
  public dataSource = new MatTableDataSource<any>([]);
  public columnas = ['expediente', 'postulante', 'formacion', 'capacitacion', 'experiencia', 'total', 'acciones'];

  public mostrarPaginador = computed(() => {
    return this.plazaSeleccionada() !== null && this.dataSource.filteredData.length > 0;
  });

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
    this.filtroTexto.set('');
    this.cargarCandidatos(idPlaza);
  }

  cargarCandidatos(idPlaza: number): void {
    this.cargando.set(true);
    this.comiteService.listarCandidatosCurricular(idPlaza).subscribe({
      next: (res) => {
        if (res.success) {
          // Re-instanciación reactiva zoneless segura
          this.dataSource = new MatTableDataSource<any>(res.data);

          if (this.paginator()) {
            this.dataSource.paginator = this.paginator()!;
          }

          if (this.filtroTexto()) {
            this.dataSource.filter = this.filtroTexto().trim().toLowerCase();
          }
        }
        this.cargando.set(false);
      },
      error: () => this.cargando.set(false)
    });
  }

  guardarEvaluacion(element: any): void {
    // Validaciones básicas antes de enviar
    if (element.notaFormacion < 0 || element.notaCapacitacion < 0 || element.notaExperiencia < 0) {
      this.alertService.error('Error', 'Los puntajes asignados no pueden ser negativos.');
      return;
    }

    this.alertService.confirmacion('¿Registrar Evaluación Curricular?', 'Una vez guardado, el estado final del postulante se actualizará de forma permanente.').subscribe(confirmado => {
      if (confirmado) {
        this.cargando.set(true);
        
        const payload = {
          idPostulacion: element.idPostulacion,
          notaFormacion: element.notaFormacion || 0,
          notaCapacitacion: element.notaCapacitacion || 0,
          notaExperiencia: element.notaExperiencia || 0
        };

        this.comiteService.guardarCalificacionCurricular(payload).subscribe({
          next: (res) => {
            if (res.success) {
              this.alertService.exito('Éxito', 'Calificación registrada correctamente.');
              this.cargarCandidatos(this.plazaSeleccionada()!); // Refrescamos grilla de inmediato
            } else {
              this.cargando.set(false);
            }
          },
          error: () => this.cargando.set(false)
        });
      }
    });
  }

  aplicarFiltroRapido(event: Event): void {
    const filterValue = (event.target as HTMLInputElement).value;
    this.filtroTexto.set(filterValue);
    this.dataSource.filter = filterValue.trim().toLowerCase();

    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
  }

  exportarReportePdf(): void {
    const idPlaza = this.plazaSeleccionada();
    if (!idPlaza) return;

    const plazaActual = this.plazas().find(p => p.idPlaza === idPlaza);
    const nombrePuesto = plazaActual ? plazaActual.nombrePuesto : idPlaza.toString();
    const nombrePuestoFormateado = nombrePuesto.toUpperCase().replace(/ /g, '_');

    this.cargando.set(true);
    
    this.comiteService.descargarActaCurricularPdf(idPlaza).subscribe({
      next: (blobData: Blob) => {
        const urlTemporal = window.URL.createObjectURL(blobData);
        const enlaceDescarga = document.createElement('a');
        enlaceDescarga.href = urlTemporal;
        enlaceDescarga.download = `Acta_Evaluacion_Curricular_${nombrePuestoFormateado}.pdf`;
        
        document.body.appendChild(enlaceDescarga);
        enlaceDescarga.click();
        document.body.removeChild(enlaceDescarga);
        window.URL.revokeObjectURL(urlTemporal);
        
        this.cargando.set(false);
      },
      error: () => {
        this.cargando.set(false);
        this.alertService.error('Error', 'No se pudo generar el acta de evaluación curricular.');
      }
    });
  }
}
