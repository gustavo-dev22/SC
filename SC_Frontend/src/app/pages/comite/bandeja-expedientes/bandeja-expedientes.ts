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

  // 🚀 Usamos un Setter en el viewChild para que en cuanto el paginador aparezca en el DOM, se enlace automáticamente
  private paginator = viewChild<MatPaginator>(MatPaginator);

  public plazas = signal<any[]>([]);
  public plazaSeleccionada = signal<number | null>(null);
  public filtroTexto = signal<string>('');
  public cargandoPlazas = signal<boolean>(false);
  public cargando = signal<boolean>(false);
  public totalRegistros = signal<number>(0);

  // 🚀 INSTANCIA ÚNICA: El dataSource se crea una sola vez en la vida del componente
  public dataSource = new MatTableDataSource<any>([]);
  public columnas = ['expediente', 'postulante', 'plaza', 'fecha', 'acciones'];

  public mostrarPaginador = computed(() => {
    return this.plazaSeleccionada() !== null && this.totalRegistros() > 0;
  });

  ngOnInit(): void {
    this.cargarPlazasVigentes();
    
    // 🚀 Vinculamos el paginador de forma reactiva al inicializar la pantalla
    // Esto asegura que use la misma instancia del dataSource pase lo que pase
    this.comiteService.listarPlazasAsignadasComite().subscribe(() => {
      setTimeout(() => {
        if (this.paginator()) {
          this.dataSource.paginator = this.paginator()!;
        }
      }, 0);
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
    
    // 🚀 En lugar de crear un nuevo DataSource, LIMPIAMOS la data de la instancia existente
    this.dataSource.data = [];
    this.totalRegistros.set(0);
    
    this.cargarExpedientes(idPlaza);
  }

  cargarExpedientes(idPlaza: number): void {
    this.cargando.set(true);
    this.comiteService.listarInscritos(idPlaza).subscribe({
      next: (res) => {
        if (res.success && res.data) {
          // 🚀 CLAVE: Seteamos la data sobre la misma instancia compartida
          this.dataSource.data = res.data;
          this.totalRegistros.set(res.data.length);
          
          if (this.filtroTexto()) {
            this.dataSource.filter = this.filtroTexto().trim().toLowerCase();
            this.totalRegistros.set(this.dataSource.filteredData.length);
          }

          // Garantizamos el amarre y refrescamos la primera página
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
            this.alertService.error('Error en Operación', err.error?.message || 'No se pudo comunicar el cambio al servidor central.');
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
    this.comiteService.descargarActaInicialPdf(idPlaza).subscribe({
      next: (blobData: Blob) => {
        const urlTemporal = window.URL.createObjectURL(blobData);
        const enlaceDescarga = document.createElement('a');
        enlaceDescarga.href = urlTemporal;
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
