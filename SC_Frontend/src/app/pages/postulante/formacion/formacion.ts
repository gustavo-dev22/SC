import { CommonModule } from '@angular/common';
import { Component, inject, Input, OnInit, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { ModalFormacion } from './modal-formacion/modal-formacion';
import { PostulanteFormacionService } from '../../../services/postulante-formacion.service';
import { MatTooltipModule } from '@angular/material/tooltip';
import { AlertService } from '../../../shared/services/alert.service';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { environment } from '../../../../environments/environment';
import { PostulanteResumenService } from '../../../services/postulante-resumen.service';

@Component({
  selector: 'app-formacion',
  imports: [CommonModule, MatDialogModule, MatButtonModule, MatIconModule, MatTooltipModule, MatProgressSpinnerModule],
  templateUrl: './formacion.html',
  styleUrl: './formacion.css',
})
export class Formacion implements OnInit{
  @Input() modoLectura = false;
  private dialog = inject(MatDialog);
  private formacionService = inject(PostulanteFormacionService);
  private _postulacionService = inject(PostulanteResumenService);
  private alertService = inject(AlertService);
  public cargando = signal(false);
  public listaFormacion = signal<any[]>([]);
  private idPostulante!: number;

  public get idEstadoPostulacionActual(): number {
    return this._postulacionService.estadoPostulacion() ?? 0;
  }

  ngOnInit(): void {
    const profile = JSON.parse(sessionStorage.getItem('user_profile') || '{}');
    const tokenParts = atob(profile.token).split('-');
    this.idPostulante = Number(tokenParts[1]);
    this.cargarFormacion();
  }

  public onFileSelected(event: Event, idFormacion: number): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      const file = input.files[0];

      // 1. Validación de seguridad básica de UX: Solo PDFs
      if (file.type !== 'application/pdf') {
        this.alertService.error('Error', 'Solo se admiten documentos en formato PDF.');
        return;
      }

      // 2. Control de peso (Ejemplo: Max 4MB para no saturar base de datos/storage)
      if (file.size > 4 * 1024 * 1024) {
        this.alertService.error('Error', 'El archivo excede el límite permitido de 4MB.');
        return;
      }

      this.subirArchivoSustentatorio(file, idFormacion);
    }
  }

  private subirArchivoSustentatorio(file: File, idFormacion: number): void {
    this.cargando.set(true);
    
    const formData = new FormData();
    formData.append('archivo', file);
    formData.append('idFormacion', idFormacion.toString());

    this.formacionService.SubirPdfSustento(formData).subscribe({
      next: (res) => {
        if (res.success) {
          this.alertService.exito('Éxito', 'Documento sustentatorio guardado con éxito.');
          
          // 🚀 CRUCIAL: Volvemos a consultar a la BD para que traiga la nueva 'rutaSustento'
          this.cargarFormacion(); 
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

    // 🚀 Extraemos la raíz del backend removiendo el segmento '/api' de la URL base
    // Si tu apiUrl es "http://localhost:5000/api", quedará como "http://localhost:5000"
    const urlBackend = environment.apiUrl.replace('/api', '');
    
    // Concatenamos la raíz con la ruta relativa (/uploads/sustentos/archivo.pdf)
    const urlCompleta = `${urlBackend}${url}`;

    window.open(urlCompleta, '_blank');
  }

  public eliminarPdf(idFormacion: number): void {
    // 🚀 1. Nos suscribimos al Observable que retorna el modal de confirmación
    this.alertService.confirmacion(
      'Mensaje de Confirmación', 
      '¿Está seguro de eliminar el documento sustentatorio cargado? Deberá subir uno nuevo para la fase curricular.', 
      'SI', 
      'NO'
    ).subscribe((confirmado: boolean) => {
      
      // 🚀 2. Evaluamos la respuesta real emitida al cerrar el modal
      if (confirmado) {
        this.cargando.set(true);
        
        this.formacionService.EliminarPdfSustento(idFormacion).subscribe({
          next: (res) => {
            this.alertService.exito('Éxito', 'El archivo sustentatorio fue removido con éxito.');
            this.cargarFormacion(); // Refresca la lista y actualiza los íconos
          },
          error: () => {
            this.cargando.set(false);
            this.alertService.error('Error', 'No se pudo procesar la eliminación del archivo.');
          }
        });
      }

    });
  }

  cargarFormacion(): void {
    this.cargando.set(true); 
    this.formacionService.getFormacion(this.idPostulante).subscribe({
      next: (res) => {
        if (res.success) this.listaFormacion.set(res.data);
        this.cargando.set(false); 
      },
      error: () => this.cargando.set(false)
    });
  }

  abrirModal(elemento: any = null): void {
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

  eliminar(id: number): void {
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
