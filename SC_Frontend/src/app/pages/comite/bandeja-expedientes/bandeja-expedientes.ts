import { Component, OnInit, computed, inject, signal, viewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ComiteEvaluacionService } from '../../../services/comite-evaluacion.service';
import { AlertService } from '../../../shared/services/alert.service';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';

@Component({
  selector: 'app-bandeja-expedientes',
  standalone: true,
  imports: [CommonModule, MatTableModule, MatButtonModule, MatIconModule, MatProgressSpinnerModule, MatPaginatorModule, MatInputModule, MatSelectModule, MatFormFieldModule, FormsModule, MatCardModule],
  templateUrl: './bandeja-expedientes.html',
  styleUrl: './bandeja-expedientes.css',
})
export class BandejaExpedientes implements OnInit {
  private comiteService = inject(ComiteEvaluacionService);
  private alertService = inject(AlertService);

  private paginator = viewChild(MatPaginator);

  public plazas = signal<any[]>([]);
  public plazaSeleccionada = signal<number | null>(null);
  public filtroTexto = signal<string>('');
  public cargandoPlazas = signal<boolean>(false);
  public dataSource = new MatTableDataSource<any>([]);

  public expedientes = signal<any[]>([]);
  public columnas = ['expediente', 'postulante', 'plaza', 'fecha', 'acciones'];
  public cargando = signal<boolean>(false);

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
    this.filtroTexto.set(''); // Reseteamos el filtro de texto al cambiar de plaza
    this.cargarExpedientes(idPlaza);
  }

  cargarExpedientes(idPlaza: number): void {
    this.cargando.set(true);
    this.comiteService.listarInscritos(idPlaza).subscribe({
      next: (res) => {
        if (res.success) {
          // 🚀 SOLUCIÓN ZONELESS: Reinstanciamos por completo el objeto dataSource
          this.dataSource = new MatTableDataSource<any>(res.data);
          
          // Volvemos a amarrar el paginador a la nueva instancia
          if (this.paginator()) {
            this.dataSource.paginator = this.paginator()!;
          }

          // Si el usuario tenía algo escrito en el buscador, re-aplicamos el filtro
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

  procesarExpediente(idPostulacion: number, aprobado: boolean): void {
    const accionTexto = aprobado ? 'Admitir para el Examen de Conocimientos' : 'Descalificar';
    const botonOkTexto = aprobado ? 'Sí, Admitir' : 'Sí, Rechazar';
    
    this.alertService.confirmacion(
      '¿Está seguro?', 
      `Va a ${accionTexto} este expediente.`, 
      botonOkTexto, 
      'Cancelar'
    ).subscribe((confirmado: boolean) => {
      if (confirmado) {
        
        this.cargando.set(true);
        
        this.comiteService.evaluarExpediente(idPostulacion, aprobado, 'Validación inicial de requisitos obligatorios.').subscribe({
          next: (res) => {
            this.cargando.set(false);

            if (res && (res.success || res.idPostulacion || res.mensaje)) { 
              this.alertService.exito('Éxito', 'Expediente procesado correctamente.');
              
              if (this.plazaSeleccionada()) {
                this.cargarExpedientes(this.plazaSeleccionada()!);
              }
            } else {
              this.alertService.error('Atención', 'El servidor procesó la solicitud pero no reportó éxito legítimo.');
            }
          },
          error: (err) => {
            this.cargando.set(false);
            
            this.alertService.error(
              'Error en Operación', 
              err.error?.message || 'No se pudo comunicar el cambio al servidor central.'
            );
          }
        });
      }
    });
  }

  exportarReportePdf(): void {
    const idPlaza = this.plazaSeleccionada();
    if (!idPlaza) return;

    // 🚀 1. Buscamos el objeto de la plaza actual en nuestra Signal en memoria
    const plazaActual = this.plazas().find(p => p.idPlaza === idPlaza);
    
    // Si por algún motivo no lo encuentra, dejamos el idPlaza como fail-safe
    const nombrePuesto = plazaActual ? plazaActual.nombrePuesto : idPlaza.toString();
    
    // 🚀 2. Normalizamos el nombre del puesto: pasamos a Mayúsculas y cambiamos espacios por guiones bajos
    const nombrePuestoFormateado = nombrePuesto.toUpperCase().replace(/ /g, '_');

    this.cargando.set(true);
    this.comiteService.descargarActaInicialPdf(idPlaza).subscribe({
      next: (blobData: Blob) => {
        const urlTemporal = window.URL.createObjectURL(blobData);
       
        const enlaceDescarga = document.createElement('a');
        enlaceDescarga.href = urlTemporal;
        
        // 🚀 3. Reemplazamos el ID de la plaza por el nombre formateado en mayúsculas
        enlaceDescarga.download = `Acta_Filtro_Inicial_Plaza_${nombrePuestoFormateado}.pdf`;
       
        document.body.appendChild(enlaceDescarga);
        enlaceDescarga.click();
        document.body.removeChild(enlaceDescarga);
        window.URL.revokeObjectURL(urlTemporal);
       
        this.cargando.set(false);
      },
      error: () => {
        this.cargando.set(false);
        this.alertService.error('Error', 'No se pudo estructurar el informe PDF de la convocatoria.');
      }
    });
  }
}
