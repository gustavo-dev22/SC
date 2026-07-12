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

  private paginator = viewChild(MatPaginator);

  public plazas = signal<any[]>([]);
  public plazaSeleccionada = signal<number | null>(null);
  public filtroTexto = signal<string>('');
  public cargandoPlazas = signal<boolean>(false);
  public cargando = signal<boolean>(false);
  
  public candidatosSignal = signal<any[]>([]);
  public dataSource = new MatTableDataSource<any>([]);
  public columnas = ['expediente', 'postulante', 'nota', 'acciones'];

  public notaMinimaExamen = signal<number>(28);
  public notaMaximaExamen = signal<number>(40);

  public mostrarPaginador = computed(() => {
    return this.plazaSeleccionada() !== null && this.dataSource.filteredData.length > 0;
  });

  ngOnInit(): void {
    this.cargarPlazasVigentes();
    this.cargarLimitesConfiguracion();
  }

  cargarLimitesConfiguracion(): void {
    // Solicitamos dinámicamente el valor máximo
    this.parametroService.getParametros('NOTA_MAXIMA_EC').subscribe({
      next: (res) => {
        if (res && res.success && res.data && res.data.length > 0) {
          this.notaMaximaExamen.set(Number(res.data[0].valor));
        }
      },
      error: () => this.notaMaximaExamen.set(40) // Resguardo fail-safe
    });

    // Solicitamos dinámicamente el valor mínimo
    this.parametroService.getParametros('NOTA_MINIMA_EC').subscribe({
      next: (res) => {
        if (res && res.success && res.data && res.data.length > 0) {
          this.notaMinimaExamen.set(Number(res.data[0].valor));
        }
      },
      error: () => this.notaMinimaExamen.set(28) // Resguardo fail-safe
    });
  }

  cargarPlazasVigentes(): void {
    this.cargandoPlazas.set(true);
    this.comiteService.listarPlazasAsignadasComite().subscribe({
      next: (res) => {
        if (res && res.content) {
          this.plazas.set(res.content);
        }
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
    this.comiteService.listarCandidatosExamen(idPlaza).subscribe({
      next: (res) => {
        if (res.success) {
          // 🚀 SOLUCIÓN: Reinstanciamos por completo el objeto para asegurar reactividad pura
          this.candidatosSignal.set(res.data);
          this.dataSource = new MatTableDataSource<any>(res.data);

          // Amarre seguro del paginador
          if (this.paginator()) {
            this.dataSource.paginator = this.paginator()!;
          }

          // Si el usuario tenía algo escrito en el buscador, re-aplicamos el filtro activo
          if (this.filtroTexto()) {
            this.dataSource.filter = this.filtroTexto().trim().toLowerCase();
          }
        }
        this.cargando.set(false);
      },
      error: () => this.cargando.set(false)
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

  guardarCalificacion(element: any): void {
    const nota = element.notaConocimientos;
    const min = this.notaMinimaExamen();
    const max = this.notaMaximaExamen();

    if (nota === null || nota === undefined) {
      this.alertService.advertencia('Nota Inválida', `Debe ingresar un puntaje válido.`);
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
            
            // 🚀 CORREGIDO: Evaluamos de forma segura si la respuesta contiene '.success' o viene como boolean directo
            const operacionExitosa = (res && typeof res === 'object') ? res.success : res;

            if (operacionExitosa) {
              this.alertService.exito('Éxito', 'Calificación oficial procesada de manera conforme.');
              
              if (this.plazaSeleccionada()) {
                // Forzamos la recarga. El Stored Procedure actualizado devolverá el registro 
                // con 'FaseConocimientosAprobado' cargado en 1 o 0, bloqueando el input automáticamente
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
    
    // Llama a tu endpoint en el servicio (debes añadirlo a tu comite-evaluacion.service.ts apuntando al nuevo endpoint BLOB)
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
