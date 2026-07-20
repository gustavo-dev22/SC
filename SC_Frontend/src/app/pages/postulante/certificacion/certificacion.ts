import { CommonModule } from '@angular/common';
import { Component, inject, Input, OnInit, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { PostulanteCertificacionService } from '../../../services/postulante-certificacion.service';
import { ModalCertificacion } from './modal-certificacion/modal-certificacion';
import { AlertService } from '../../../shared/services/alert.service';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { PostulacionService } from '../../../services/postulacion.service';
import { environment } from '../../../../environments/environment';
import { DocumentoSustentoService } from '../../../services/documento-sustento.service';
import { ParametroService } from '../../../services/parametro.service';
import { forkJoin } from 'rxjs';

@Component({
  selector: 'app-certificacion',
  imports: [CommonModule, MatDialogModule, MatButtonModule, MatIconModule, MatTooltipModule, MatProgressSpinnerModule],
  templateUrl: './certificacion.html',
  styleUrl: './certificacion.css',
})
export class Certificacion implements OnInit {
  @Input() modoLectura = false;
  private dialog = inject(MatDialog);
  private certService = inject(PostulanteCertificacionService);
  private _postulacionService = inject(PostulacionService);
  private documentoSustentoService = inject(DocumentoSustentoService);
  private alertService = inject(AlertService);
  private parametroService = inject(ParametroService);

  public listaCertificaciones = signal<any[]>([]);
  private idPostulante!: number;
  public cargando = signal<boolean>(false);
  public limiteArchivoMb = signal<number>(5);

  private readonly CONTROLADOR = 'postulantecertificacion';

  public get idEstadoPostulacionActual(): number {
    return this._postulacionService.estadoPostulacion() ?? 0;
  }

  ngOnInit(): void {
    const profile = JSON.parse(sessionStorage.getItem('user_profile') || '{}');
    const tokenParts = atob(profile.token).split('-');
    this.idPostulante = Number(tokenParts[1]);
    this.inicializarModuloCertificaciones();
  }

  private inicializarModuloCertificaciones(): void {
    this.cargando.set(true);

    forkJoin({
      certificaciones: this.certService.getCertificaciones(this.idPostulante),
      parametroSize: this.parametroService.getParametros('MAX_FILE_SIZE_MB')
    }).subscribe({
      next: (resultado) => {
        // 1. Poblamos el listado de capacitaciones
        if (resultado.certificaciones.success) {
          this.listaCertificaciones.set(resultado.certificaciones.data);
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
        // Fail-safe activo: conserva los 4MB por defecto si el API de parámetros falla
      }
    });
  }

  cargarCertificaciones(): void {
    this.cargando.set(true); 
    this.certService.getCertificaciones(this.idPostulante).subscribe({
      next: (res) => {
        if (res.success) this.listaCertificaciones.set(res.data);
        this.cargando.set(false); 
      },
      error: () => this.cargando.set(false)
    });
  }

  public onFileSelected(event: Event, idCertificacion: number): void {
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
          `El archivo pesa más de lo permitido. El tamaño máximo configurado para cargar sus sustentos es de ${this.limiteArchivoMb()}MB.`
        );
        return;
      }

      this.subirArchivoSustentatorio(file, idCertificacion);
    }
  }

  private subirArchivoSustentatorio(file: File, idCertificacion: number): void {
    this.cargando.set(true);
    
    // 🚀 Consumimos el servicio universal pasándole las credenciales de la sección
    this.documentoSustentoService.subirPdf(this.CONTROLADOR, idCertificacion, 'idCertificacion', file).subscribe({
      next: (res) => {
        if (res.success) {
          this.alertService.exito('Éxito', 'Documento de certificación guardado con éxito.');
          this.cargarCertificaciones(); 

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

  public eliminarPdf(idCertificacion: number): void {
    this.alertService.confirmacion(
      'Mensaje de Confirmación', 
      '¿Está seguro de eliminar el documento sustentatorio cargado? Deberá subir uno nuevo para la fase curricular.', 
      'SI', 
      'NO'
    ).subscribe((confirmado: boolean) => {
      if (confirmado) {
        this.cargando.set(true);
        
        // 🚀 Consumimos la eliminación universal
        this.documentoSustentoService.eliminarPdf(this.CONTROLADOR, idCertificacion).subscribe({
          next: (res) => {
            this.alertService.exito('Éxito', 'El archivo sustentatorio fue removido con éxito.');
            this.cargarCertificaciones(); 
            
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

  abrirModal(elemento: any = null): void {
    const dialogRef = this.dialog.open(ModalCertificacion, {
      panelClass: 'custom-academic-dialog-panel',
      disableClose: true,
      autoFocus: 'first-tabbable',
      data: { elemento }
    });

    dialogRef.afterClosed().subscribe(payload => {
      if (payload) {
        payload.idPostulante = this.idPostulante;
        
        this.cargando.set(true); // 🚀 Enciende durante la actualización en backend (.NET)
        this.certService.mantenimiento(payload).subscribe({
          next: () => {
            this.alertService.exito('¡Éxito!', 'Certificación actualizada correctamente.');
            this.cargarCertificaciones(); // El cargador del listado se encargará de apagar el spinner
          },
          error: () => this.cargando.set(false)
        });
      }
    });
  }

  eliminar(id: number): void {
    this.alertService.confirmacion(
      '¿Retirar Certificación?', 
      'Esta acción eliminará de forma permanente el registro de su capacitación. ¿Desea continuar?'
    ).subscribe(confirmado => {
      if (confirmado) {
        const payload = { accion: 'ELIMINAR', idCertificacion: id, idPostulante: this.idPostulante, idTipoEstudioCat:0, nombreEstudio:'', institucion:'', horasAcademicas:0, fechaEmision: new Date() };
        
        this.cargando.set(true);
        this.certService.mantenimiento(payload).subscribe({
          next: () => {
            this.cargarCertificaciones();
          },
          error: () => this.cargando.set(false)
        });
      }
    });
  }
}
