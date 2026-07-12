import { Component, OnInit, inject, signal, viewChild, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatCardModule } from '@angular/material/card';
import { ComiteEvaluacionService } from '../../../services/comite-evaluacion.service';
import { AlertService } from '../../../shared/services/alert.service';
import { ParametroService } from '../../../services/parametro.service';

@Component({
  selector: 'app-evaluacion-conocimientos',
  standalone: true,
  imports: [
    CommonModule, 
    FormsModule, 
    MatTableModule, 
    MatButtonModule, 
    MatIconModule, 
    MatProgressSpinnerModule, 
    MatPaginatorModule, 
    MatInputModule, 
    MatSelectModule, 
    MatFormFieldModule, 
    MatCardModule
  ],
  templateUrl: './evaluacion-conocimientos.html',
  styleUrl: './evaluacion-conocimientos.css'
})
export class EvaluacionConocimientos implements OnInit {
  private comiteService = inject(ComiteEvaluacionService);
  private alertService = inject(AlertService);
  private parametroService = inject(ParametroService);

  private paginator = viewChild<MatPaginator>(MatPaginator);

  public plazas = signal<any[]>([]);
  public plazaSeleccionada = signal<number | null>(null);
  public filtroTexto = signal<string>('');
  public cargandoPlazas = signal<boolean>(false);
  public cargando = signal<boolean>(false);
  public totalRegistros = signal<number>(0);
  
  // 🚀 INSTANCIA ESTABLE ÚNICA: Evita que el paginador pierda su referencia en el DOM
  public dataSource = new MatTableDataSource<any>([]);
  public columnas = ['expediente', 'postulante', 'nota', 'acciones'];

  public notaMinimaExamen = signal<number>(28);
  public notaMaximaExamen = signal<number>(40);

  public mostrarPaginador = computed(() => {
    return this.plazaSeleccionada() !== null && this.totalRegistros() > 0;
  });

  ngOnInit(): void {
    this.cargarPlazasVigentes();
    this.cargarLimitesConfiguracion();

    // Amarre primario seguro
    this.comiteService.listarPlazasAsignadasComite().subscribe(() => {
      setTimeout(() => {
        if (this.paginator()) {
          this.dataSource.paginator = this.paginator()!;
        }
      }, 0);
    });
  }

  cargarLimitesConfiguracion(): void {
    this.parametroService.getParametros('NOTA_MAXIMA_EC').subscribe({
      next: (res) => {
        if (res && res.success && res.data && res.data.length > 0) {
          this.notaMaximaExamen.set(Number(res.data[0].valor));
        }
      },
      error: () => this.notaMaximaExamen.set(40)
    });

    this.parametroService.getParametros('NOTA_MINIMA_EC').subscribe({
      next: (res) => {
        if (res && res.success && res.data && res.data.length > 0) {
          this.notaMinimaExamen.set(Number(res.data[0].valor));
        }
      },
      error: () => this.notaMinimaExamen.set(28)
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
    
    // 🚀 Limpieza instantánea para forzar la desaparición inmediata del paginador
    this.dataSource.data = [];
    this.totalRegistros.set(0);
    
    this.cargarCandidatos(idPlaza);
  }

  cargarCandidatos(idPlaza: number): void {
    this.cargando.set(true);
    this.comiteService.listarCandidatosExamen(idPlaza).subscribe({
      next: (res) => {
        if (res.success && res.data) {
          // 🚀 Cargamos la data sobre la instancia ya compartida
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

  aplicarFiltroRapido(event: Event): void {
    const filterValue = (event.target as HTMLInputElement).value;
    this.filtroTexto.set(filterValue);
    this.dataSource.filter = filterValue.trim().toLowerCase();
    this.totalRegistros.set(this.dataSource.filteredData.length);

    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
  }

  guardarCalificacion(element: any): void {
    const nota = element.notaConocimientos;
    const min = this.notaMinimaExamen();
    const max = this.notaMaximaExamen();

    if (nota === null || nota === undefined || nota < min || nota > max) {
      this.alertService.advertencia('Nota Inválida', `Debe ingresar un puntaje válido entre ${min} y ${max}.`);
      return;
    }

    this.alertService.confirmacion(
      '¿Registrar Calificación?',
      `Se grabará la nota de ${Number(nota).toFixed(2)} para el postulante.`,
      'Sí, Grabar',
      'Cancelar'
    ).subscribe((confirmado: boolean) => {
      if (confirmado) {
        this.cargando.set(true);
        const payload = {
          idPostulacion: Number(element.idPostulacion),
          notaConocimientos: Number(nota)
        };

        this.comiteService.registrarNotaExamen(payload).subscribe({
          next: (res) => {
            this.cargando.set(false);
            const operacionExitosa = (res && typeof res === 'object') ? res.success : res;

            if (operacionExitosa) {
              this.alertService.exito('Éxito', 'Calificación oficial procesada de manera conforme.');
              if (this.plazaSeleccionada()) {
                this.cargarCandidatos(this.plazaSeleccionada()!);
              }
            } else {
              this.alertService.error('Error', 'El servidor no pudo procesar el cambio de estado del expediente.');
            }
          },
          error: (err) => {
            this.cargando.set(false);
            this.alertService.error('Error', 'Ocurrió un fallo de red al comunicar la calificación.');
            console.error("❌ ERROR AL REGISTRAR:", err);
          }
        });
      }
    });
  }

  exportarReportePdf(): void {
    const idPlaza = this.plazaSeleccionada();
    if (!idPlaza) return;

    const plazaActual = this.plazas().find(p => p.idPlaza === idPlaza);
    const nombrePuesto = plazaActual ? plazaActual.nombrePuesto : idPlaza.toString();
    const nombrePuestoFormateado = nombrePuesto.toUpperCase().replace(/ /g, '_');

    this.cargando.set(true);
    this.comiteService.descargarActaConocimientosPdf(idPlaza).subscribe({
      next: (blobData: Blob) => {
        const urlTemporal = window.URL.createObjectURL(blobData);
        const enlaceDescarga = document.createElement('a');
        enlaceDescarga.href = urlTemporal;
        enlaceDescarga.download = `Acta_Examen_Conocimientos_${nombrePuestoFormateado}.pdf`;
        
        document.body.appendChild(enlaceDescarga);
        enlaceDescarga.click();
        document.body.removeChild(enlaceDescarga);
        window.URL.revokeObjectURL(urlTemporal);
        
        this.cargando.set(false);
      },
      error: () => {
        this.cargando.set(false);
        this.alertService.error('Error', 'No se pudo generar el acta de conocimientos.');
      }
    });
  }
}
