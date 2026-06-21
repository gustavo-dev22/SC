import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { OportunidadesService } from '../../services/oportunidades.service';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { AlertService } from '../../shared/services/alert.service';
import { MatTooltipModule } from '@angular/material/tooltip';

@Component({
  selector: 'app-resumen-postulaciones',
  standalone: true,
  imports: [CommonModule, MatTableModule, MatIconModule, MatButtonModule, MatProgressSpinnerModule, MatTooltipModule],
  templateUrl: './resumen-postulaciones.html',
  styleUrls: ['./resumen-postulaciones.css']
})
export class ResumenPostulaciones implements OnInit {
  private os = inject(OportunidadesService);
  private alertService = inject(AlertService);

  public postulaciones = signal<any[]>([]);
  public cargando = signal<boolean>(false);
  public columnasClave: string[] = ['codigoPostulacion', 'convocatoria', 'puesto', 'fecha', 'remuneracion', 'estado', 'acciones'];
  
  private idPostulante!: number;

  ngOnInit(): void {
    const profile = JSON.parse(sessionStorage.getItem('user_profile') || '{}');
    const tokenParts = atob(profile.token).split('-');
    this.idPostulante = Number(tokenParts[1]);
    this.cargarHistorial();
  }

  cargarHistorial(): void {
    this.cargando.set(true);
    this.os.obtenerMisPostulaciones(this.idPostulante).subscribe({
      next: (res) => {
        if (res.success) {
          this.postulaciones.set(res.data);
        }
        this.cargando.set(false);
      },
      error: () => this.cargando.set(false)
    });
  }

  // 🚀 CONTROLADOR DE ESTILOS: Retorna clases CSS según los códigos de tu catálogo valor
  obtenerClaseEstado(codigo: string): string {
    switch (codigo) {
      case 'INS': return 'badge-inscrito';       // Azul / Turquesa
      case 'APT_CV': return 'badge-apto';       // Verde institucional
      case 'NO_APT': return 'badge-noapto';     // Rojo / Gris oscuro
      case 'GAN': return 'badge-ganador';       // Dorado / Amarillo Premium
      default: return 'badge-defecto';
    }
  }

  descargarConstanciaPDF(postulacion: any): void {
    // Encendemos el spinner homogeneizado
    this.cargando.set(true);

    this.os.imprimirConstanciaReporte(postulacion.idPostulacion).subscribe({
      next: (blob: Blob) => {
        // Forzamos el tipo MIME correcto
        const pdfBlob = new Blob([blob], { type: 'application/pdf' });
        const fileURL = URL.createObjectURL(pdfBlob);
        
        // Creamos el ancla temporal para la descarga nativa
        const link = document.createElement('a');
        link.href = fileURL;
        link.target = '_blank';
        
        // Usamos el código oficial de postulación (ej: POST-2026-00045) para bautizar el archivo
        link.download = `Constancia_${postulacion.codigoPostulacion}.pdf`;
        
        // Gatillamos la acción en el DOM de manera transparente
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);

        // Liberación de memoria en segundo plano
        setTimeout(() => URL.revokeObjectURL(fileURL), 10000);
        this.cargando.set(false);
      },
      error: () => {
        this.cargando.set(false);
        this.alertService.error('Error', 'No se pudo generar la constancia de postulación.');
      }
    });
  }
}
