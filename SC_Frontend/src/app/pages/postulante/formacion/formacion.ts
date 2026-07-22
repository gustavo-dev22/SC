import { Component, Input, OnInit, inject, signal } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { PostulanteFormacionService } from '../../../services/postulante-formacion.service'; // Adapta tus rutas
import { PostulacionService } from '../../../services/postulacion.service'; 
import { AlertService } from '../../../shared/services/alert.service';
import { environment } from '../../../../environments/environment';
import { ModalFormacion } from './modal-formacion/modal-formacion';
import { CommonModule } from '@angular/common';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { DocumentoSustentoService } from '../../../services/documento-sustento.service';
import { ParametroService } from '../../../services/parametro.service';
import { forkJoin } from 'rxjs';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-formacion',
  imports: [CommonModule, MatProgressSpinnerModule, MatButtonModule, MatIconModule, MatTooltipModule],
  templateUrl: './formacion.html',
  styleUrls: ['./formacion.css']
})

export class Formacion implements OnInit {
  @Input() modoLectura = false;
  private dialog = inject(MatDialog);
  private formacionService = inject(PostulanteFormacionService);
  private _postulacionService = inject(PostulacionService);
  private alertService = inject(AlertService);
  private documentoSustentoService = inject(DocumentoSustentoService);
  private parametroService = inject(ParametroService);
  private authService = inject(AuthService);

  public cargando = signal(false);
  public listaFormacion = signal<any[]>([]);
  public limiteArchivoMb = signal<number>(5);
  private idPostulante!: number;

  private readonly CONTROLADOR = 'postulanteformacion';

  public get idEstadoPostulacionActual(): number {
    return this._postulacionService.estadoPostulacion() ?? 0;
  }

  ngOnInit(): void {
    this.idPostulante = this.authService.obtenerIdPostulanteDesdeJwt();
    
    if (this.idPostulante > 0) {
      this.inicializarModuloFormacion();
    } else {
      this.alertService.error('Error de Sesión', 'No se pudo identificar al postulante. Por favor reinicie sesión.');
    }
  }

  // 🚀 MEJORA: Cargamos la data académica y la política del tamaño del archivo en paralelo
  private inicializarModuloFormacion(): void {
    this.cargando.set(true);

    forkJoin({
      formacion: this.formacionService.getFormacion(this.idPostulante),
      parametroSize: this.parametroService.getParametros('MAX_FILE_SIZE_MB')
    }).subscribe({
      next: (resultado) => {
        // 1. Cargamos el historial académico
        if (resultado.formacion.success) {
          this.listaFormacion.set(resultado.formacion.data);
        }

        // 2. Cargamos el límite dinámico de la base de datos
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
        // Si hay error de comunicación, el fail-safe mantiene los 4MB activos por seguridad
      }
    });
  }

  public onFileSelected(event: Event, idFormacion: number): void {
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
          `El archivo pesa más de lo permitido. El tamaño máximo configurado para su postulación es de ${this.limiteArchivoMb()}MB.`
        );
        return;
      }

      this.subirArchivoSustentatorio(file, idFormacion);
    }
  }

  private subirArchivoSustentatorio(file: File, idFormacion: number): void {
    this.cargando.set(true);
    
    // 🚀 Consumimos el servicio universal pasándole las credenciales de la sección
    this.documentoSustentoService.subirPdf(this.CONTROLADOR, idFormacion, 'idFormacion', file).subscribe({
      next: (res) => {
        if (res.success) {
          this.alertService.exito('Éxito', 'Documento de formación académica guardado con éxito.');
          this.cargarFormacion(); 

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

  public eliminarPdf(idFormacion: number): void {
    this.alertService.confirmacion(
      'Mensaje de Confirmación', 
      '¿Está seguro de eliminar el documento sustentatorio cargado? Deberá subir uno nuevo para la fase curricular.', 
      'SI', 
      'NO'
    ).subscribe((confirmado: boolean) => {
      if (confirmado) {
        this.cargando.set(true);
        
        // 🚀 Consumimos la eliminación universal
        this.documentoSustentoService.eliminarPdf(this.CONTROLADOR, idFormacion).subscribe({
          next: (res) => {
            this.alertService.exito('Éxito', 'El archivo sustentatorio fue removido con éxito.');
            this.cargarFormacion(); 
            
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

  public cargarFormacion(): void {
    this.cargando.set(true); 
    this.formacionService.getFormacion(this.idPostulante).subscribe({
      next: (res) => {
        if (res.success) this.listaFormacion.set(res.data);
        this.cargando.set(false); 
      },
      error: () => this.cargando.set(false)
    });
  }

  public abrirModal(elemento: any = null): void {
    const dialogRef = this.dialog.open(ModalFormacion, { 
      width: '560px', 
      maxWidth: '95vw', 
      disableClose: true, 
      panelClass: 'custom-academic-dialog-panel', 
      data: { elemento } 
    });

    dialogRef.afterClosed().subscribe(payload => {
      if (payload) {
        payload.idPostulante = this.idPostulante;
        this.cargando.set(true); 
        this.formacionService.mantenimiento(payload).subscribe({
          next: () => {
            this.alertService.exito('¡Éxito!', 'Historial académico actualizado con éxito.');
            this.cargarFormacion(); 
          },
          error: () => this.cargando.set(false)
        });
      }
    });
  }

  public eliminar(id: number): void {
    this.alertService.confirmacion('¿Eliminar Registro?', 'Esta acción retirará de forma permanente este estudio de su currículum. ¿Desea continuar?').subscribe(confirmado => {
      if (confirmado) {
        const payload = { accion: 'ELIMINAR', idFormacion: id, idPostulante: this.idPostulante, idNivelCat: 0, idEstadoCat: 0, institucion: '', carrera: '', mesInicio: 0, anioInicio: 0 };
        this.cargando.set(true); 
        this.formacionService.mantenimiento(payload).subscribe({
          next: () => {
            this.cargarFormacion();
          },
          error: () => this.cargando.set(false)
        });
      }
    });
  }
}