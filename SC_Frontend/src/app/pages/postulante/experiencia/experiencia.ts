import { CommonModule } from '@angular/common';
import { Component, computed, inject, Input, OnInit, signal, ViewContainerRef } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { PostulanteExperienciaService } from '../../../services/postulante-experiencia.service';
import { ModalExperiencia } from './modal-experiencia/modal-experiencia';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { AlertService } from '../../../shared/services/alert.service';
import { DocumentoSustentoService } from '../../../services/documento-sustento.service';
import { PostulacionService } from '../../../services/postulacion.service';
import { environment } from '../../../../environments/environment';
import { ParametroService } from '../../../services/parametro.service';
import { forkJoin } from 'rxjs';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-experiencia',
  imports: [CommonModule, MatDialogModule, MatButtonModule, MatIconModule, MatTooltipModule, MatProgressSpinnerModule],
  templateUrl: './experiencia.html',
  styleUrl: './experiencia.css',
})
export class Experiencia implements OnInit {
  @Input() modoLectura = false;
  private dialog = inject(MatDialog);
  private expService = inject(PostulanteExperienciaService);
  private _postulacionService = inject(PostulacionService);
  private viewContainerRef = inject(ViewContainerRef);
  private alertService = inject(AlertService);
  private documentService = inject(DocumentoSustentoService);
  private SkinnerParamService = inject(ParametroService);
  private authService = inject(AuthService);

  public listaExperiencias = signal<any[]>([]);
  public cargando = signal<boolean>(false);
  public limiteArchivoMb = signal<number>(5);
  private idPostulante!: number;

  private readonly CONTROLADOR = 'postulanteexperiencia';

  public resumenGeneral = computed(() => this.convertirDiasALegible(
    this.listaExperiencias().reduce((acc, item) => acc + item.totalDiasAcumulados, 0)
  ));

  public resumenEspecifico = computed(() => this.convertirDiasALegible(
    this.listaExperiencias().filter(x => x.esExperienciaEspecifica).reduce((acc, item) => acc + item.totalDiasAcumulados, 0)
  ));

  public get idEstadoPostulacionActual(): number {
    return this._postulacionService.estadoPostulacion() ?? 0;
  }

  ngOnInit(): void {
    this.idPostulante = this.authService.obtenerIdPostulanteDesdeJwt();
    
    if (this.idPostulante > 0) {
      this.inicializarModuloExperiencia();
    } else {
      this.alertService.error('Error de Sesión', 'No se pudo identificar al postulante. Por favor reinicie sesión.');
    }
  }

  private inicializarModuloExperiencia(): void {
    this.cargando.set(true);

    forkJoin({
      experiencias: this.expService.getExperiencias(this.idPostulante),
      parametroSize: this.SkinnerParamService.getParametros('MAX_FILE_SIZE_MB')
    }).subscribe({
      next: (resultado) => {
        // 1. Poblamos la sábana de contratos
        if (resultado.experiencias.success) {
          this.listaExperiencias.set(resultado.experiencias.data);
        }

        // 2. Poblamos el límite dinámico de megabytes
        if (resultado.parametroSize.success && resultado.parametroSize.data.length > 0) {
          const valorDb = Number(resultado.parametroSize.data[0].valor);
          if (!isNaN(valorDb) && valorDb > 0) {
            this.limiteArchivoMb.set(valorDb);
          }
        }
        this.cargando.set(false);
      },
      error: () => {
        this.cargando.set(false);
        // Fail-safe activo: conserva los 4MB iniciales si la red falla
      }
    });
  }

  cargarExperiencias(): void {
    this.cargando.set(true);
    this.expService.getExperiencias(this.idPostulante).subscribe({
      next: (res) => {
        if (res.success) this.listaExperiencias.set(res.data);
        this.cargando.set(false); 
      },
      error: () => this.cargando.set(false)
    });
  }

  public onFileSelected(event: Event, idExperiencia: number): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      const file = input.files[0];
      if (file.type !== 'application/pdf') {
        this.alertService.error('Error', 'Solo se admiten documentos en formato PDF.');
        return;
      }
      const maxBytes = this.limiteArchivoMb() * 1024 * 1024;
      if (file.size > maxBytes) {
        this.alertService.error(
          'Archivo Excede el Límite', 
          `El archivo pesa más de lo permitido. El tamaño máximo configurado para cargar sus constancias es de ${this.limiteArchivoMb()}MB.`
        );
        return;
      }
      this.subirArchivoSustentatorio(file, idExperiencia);
    }
  }

  private subirArchivoSustentatorio(file: File, idExperiencia: number): void {
    this.cargando.set(true);
    
    // 🚀 Invocación universal limpia pasándole el ID parámetro 'idExperiencia'
    this.documentService.subirPdf(this.CONTROLADOR, idExperiencia, 'idExperiencia', file).subscribe({
      next: (res) => {
        if (res.success) {
          this.alertService.exito('Éxito', 'Constancia de trabajo guardada con éxito.');
          this.cargarExperiencias(); 

          const plazaActualId = this._postulacionService.plazaContextoSeleccionada();
          this._postulacionService.consultarEstadoPostulacion(plazaActualId).subscribe({
            next: () => this.cargando.set(false),
            error: () => this.cargando.set(false)
          });
        } else {
          this.cargando.set(false);
        }
      },
      error: () => {
        this.cargando.set(false);
        this.alertService.error('Error', 'Ocurrió un error al subir el archivo.');
      }
    });
  }

  public verPdf(url: string): void {
    if (!url) return;
    const urlBackend = environment.apiUrl.replace('/api', '');
    const urlCompleta = `${urlBackend}${url}`;
    window.open(urlCompleta, '_blank');
  }

  public eliminarPdf(idExperiencia: number): void {
    this.alertService.confirmacion(
      'Mensaje de Confirmación', 
      '¿Está seguro de eliminar el sustento de experiencia laboral? Deberá subir uno nuevo para la fase curricular.', 
      'SI', 
      'NO'
    ).subscribe((confirmado: boolean) => {
      if (confirmado) {
        this.cargando.set(true);
        this.documentService.eliminarPdf(this.CONTROLADOR, idExperiencia).subscribe({
          next: (res) => {
            this.alertService.exito('Éxito', 'El archivo fue removido con éxito.');
            this.cargarExperiencias(); 
            
            const plazaActualId = this._postulacionService.plazaContextoSeleccionada();
            this._postulacionService.consultarEstadoPostulacion(plazaActualId).subscribe({
              next: () => this.cargando.set(false),
              error: () => this.cargando.set(false)
            });
          },
          error: () => {
            this.cargando.set(false);
            this.alertService.error('Error', 'No se pudo procesar la eliminación del archivo.');
          }
        });
      }
    });
  }

  public convertirDiasALegible(totalDias: number): string {
    if (totalDias <= 0) return '0 días';
    
    // 🚀 CORREGIDO: En administración pública y RRHH se utiliza 
    // el año comercial de 360 días para el cómputo de tiempos.
    const anios = Math.floor(totalDias / 360);
    const diasRestantesAnio = totalDias % 360;
    
    // El mes comercial es estrictamente de 30 días
    const meses = Math.floor(diasRestantesAnio / 30);
    const dias = diasRestantesAnio % 30;

    let resultado = [];
    if (anios > 0) resultado.push(`${anios} ${anios === 1 ? 'año' : 'años'}`);
    if (meses > 0) resultado.push(`${meses} ${meses === 1 ? 'mes' : 'meses'}`);
    if (dias > 0 || resultado.length === 0) resultado.push(`${dias} ${dias === 1 ? 'día' : 'días'}`);

    return resultado.join(', ');
  }

  abrirModal(elemento: any = null): void {
    const dialogRef = this.dialog.open(ModalExperiencia, {
      panelClass: 'custom-academic-dialog-panel', 
      disableClose: true,
      autoFocus: 'first-tabbable',
      viewContainerRef: this.viewContainerRef,
      data: { elemento, listaActual: this.listaExperiencias() }
    });

    dialogRef.afterClosed().subscribe(payload => {
      if (payload) {
        payload.idPostulante = this.idPostulante;
        
        this.cargando.set(true); 
        this.expService.mantenimiento(payload).subscribe({
          next: () => {
            this.alertService.exito('¡Éxito!', 'Historial laboral actualizado correctamente.');
            this.cargarExperiencias(); 
          },
          error: () => this.cargando.set(false)
        });
      }
    });
  }

  eliminar(id: number): void {
    this.alertService.confirmacion(
      '¿Eliminar Registro Laboral?', 
      'Esta acción retirará de forma permanente este contrato de su currículum. ¿Desea continuar?'
    ).subscribe(confirmado => {
      if (confirmado) {
        const payload = { accion: 'ELIMINAR', idExperiencia: id, idPostulante: this.idPostulante, empresaInstitucion:'', cargoPuesto:'', fechaInicio: new Date(), funcionesPrincipales:'' };
        
        this.cargando.set(true); 
        this.expService.mantenimiento(payload).subscribe({
          next: () => {
            this.cargarExperiencias();
          },
          error: () => this.cargando.set(false)
        });
      }
    });
  }
}
