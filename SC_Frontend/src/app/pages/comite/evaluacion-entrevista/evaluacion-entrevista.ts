import { Component, computed, inject, OnInit, signal, viewChild } from '@angular/core';
import { ComiteEvaluacionService } from '../../../services/comite-evaluacion.service';
import { AlertService } from '../../../shared/services/alert.service';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { CommonModule } from '@angular/common';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { FormsModule } from '@angular/forms';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatCardModule } from '@angular/material/card';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { ParametroService } from '../../../services/parametro.service';

@Component({
  selector: 'app-evaluacion-entrevista',
  imports: [CommonModule, MatTableModule, MatPaginatorModule, MatFormFieldModule, MatInputModule, FormsModule, MatProgressSpinnerModule, MatCardModule, MatSelectModule, MatButtonModule, MatIconModule],
  templateUrl: './evaluacion-entrevista.html',
  styleUrl: './evaluacion-entrevista.css',
})
export class EvaluacionEntrevista implements OnInit {
  private comiteService = inject(ComiteEvaluacionService);
  private parametroService = inject(ParametroService); 
  private alertService = inject(AlertService);
  private paginator = viewChild<MatPaginator>(MatPaginator);

  public plazas = signal<any[]>([]);
  public plazaSeleccionada = signal<number | null>(null);
  public filtroTexto = signal<string>('');
  public cargandoPlazas = signal<boolean>(false);
  public cargando = signal<boolean>(false);
  public totalRegistros = signal<number>(0);
  
  // 🚀 INSTANCIA FIJA ÚNICA: Evita romper la sincronía con las directivas del paginador
  public dataSource = new MatTableDataSource<any>([]);
  public columnas = ['expediente', 'postulante', 'nota', 'acciones'];

  public notaMinimaEntrevista = signal<number>(30);
  public notaMaximaEntrevista = signal<number>(50);

  public mostrarPaginador = computed(() => {
    return this.plazaSeleccionada() !== null && this.totalRegistros() > 0;
  });

  ngOnInit(): void {
    this.cargarPlazasVigentes();
    this.cargarLimitesConfiguracion();

    // Vinculación estable inicial
    this.comiteService.listarPlazasAsignadasComite().subscribe(() => {
      setTimeout(() => {
        if (this.paginator()) {
          this.dataSource.paginator = this.paginator()!;
        }
      }, 0);
    });
  }

  cargarLimitesConfiguracion(): void {
    this.parametroService.getParametros('NOTA_MAXIMA_ENT').subscribe({
      next: (res) => {
        if (res && res.success && res.data && res.data.length > 0) {
          this.notaMaximaEntrevista.set(Number(res.data[0].valor));
        }
      },
      error: () => this.notaMaximaEntrevista.set(50)
    });

    this.parametroService.getParametros('NOTA_MINIMA_ENT').subscribe({
      next: (res) => {
        if (res && res.success && res.data && res.data.length > 0) {
          this.notaMinimaEntrevista.set(Number(res.data[0].valor));
        }
      },
      error: () => this.notaMinimaEntrevista.set(30)
    });
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
    
    // 🚀 Ocultamiento preventivo inmediato de los controles
    this.dataSource.data = [];
    this.totalRegistros.set(0);

    this.cargarCandidatos(idPlaza);
  }

  cargarCandidatos(idPlaza: number): void {
    this.cargando.set(true);
    this.comiteService.listarCandidatosEntrevista(idPlaza).subscribe({
      next: (res) => {
        if (res.success && res.data) {
          // 🚀 Poblamos sobre la instancia existente compartida
          this.dataSource.data = res.data;
          this.totalRegistros.set(res.data.length);

          if (this.filtroTexto()) {
            this.dataSource.filter = this.filtroTexto().trim().toLowerCase();
            this.totalRegistros.set(this.dataSource.filteredData.length);
          }

          if (this.paginator()) {
            this.dataSource.paginator = this.paginator()!;
            if (this.dataSource.paginator) {
              this.dataSource.paginator.firstPage();
            }
          }
        } else {
          this.dataSource.data = [];
          this.totalRegistros.set(0);
        }
        this.cargando.set(false);
      },
      error: () => {
        this.dataSource.data = [];
        this.totalRegistros.set(0);
        this.cargando.set(false);
      }
    });
  }

  guardarCalificacion(element: any): void {
    const nota = element.notaEntrevista;
    const max = this.notaMaximaEntrevista();

    if (nota === null || nota === undefined || nota < 0 || nota > max) {
      this.alertService.error('Error', `Ingrese una calificación válida entre 0 y ${max} puntos.`);
      return;
    }

    this.alertService.confirmacion('¿Registrar Nota de Entrevista?', 'Esta calificación determinará el estado de GANADOR o NO APTO del postulante.').subscribe(confirmado => {
      if (confirmado) {
        this.cargando.set(true);
        const payload = { idPostulacion: element.idPostulacion, notaEntrevista: element.notaEntrevista };
        
        this.comiteService.registrarNotaEntrevista(payload).subscribe({
          next: (res) => {
            this.cargando.set(false);
            const operacionExitosa = (res && typeof res === 'object') ? res.success : res;

            if (operacionExitosa) {
              this.alertService.exito('¡Éxito!', 'Nota de entrevista guardada con éxito.');
              if (this.plazaSeleccionada()) {
                this.cargarCandidatos(this.plazaSeleccionada()!); 
              }
            } else {
              this.alertService.error('Error', 'No se pudo procesar el guardado de la nota.');
            }
          },
          error: () => {
            this.cargando.set(false);
            this.alertService.error('Error', 'Fallo de red al registrar la calificación.');
          }
        });
      }
    });
  }

  aplicarFiltroRapido(event: Event): void {
    const filterValue = (event.target as HTMLInputElement).value;
    this.filtroTexto.set(filterValue);
    this.dataSource.filter = filterValue.trim().toLowerCase();
    this.totalRegistros.set(this.dataSource.filteredData.length);

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
    this.comiteService.descargarActaEntrevistaPdf(idPlaza).subscribe({
      next: (blobData: Blob) => {
        const urlTemporal = window.URL.createObjectURL(blobData);
        const enlaceDescarga = document.createElement('a');
        enlaceDescarga.href = urlTemporal;
        enlaceDescarga.download = `Acta_Resultados_Finales_${nombrePuestoFormateado}.pdf`;
        
        document.body.appendChild(enlaceDescarga);
        enlaceDescarga.click();
        document.body.removeChild(enlaceDescarga);
        window.URL.revokeObjectURL(urlTemporal);
        
        this.cargando.set(false);
      },
      error: () => {
        this.cargando.set(false);
        this.alertService.error('Error', 'No se pudo generar el acta de resultados finales.');
      }
    });
  }
}
