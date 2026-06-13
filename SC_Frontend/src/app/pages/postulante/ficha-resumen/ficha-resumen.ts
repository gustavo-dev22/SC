import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule, NgComponentOutlet } from '@angular/common';
import { MatTabsModule } from '@angular/material/tabs';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { PostulanteResumenService } from '../../../services/postulante-resumen.service';
import { TabConfig } from '../../../core/models/tabConfig.model';
import { AlertService } from '../../../shared/services/alert.service';

import { Formacion } from '../formacion/formacion';
import { Colegiatura } from '../colegiatura/colegiatura';
import { Idiomas } from '../idioma/idioma';
import { Ofimatica } from '../ofimatica/ofimatica';
import { Certificacion } from '../certificacion/certificacion';
import { Experiencia } from '../experiencia/experiencia';
import { OtrosRequisitos } from '../otros-requisitos/otros-requisitos';
import { InformacionAdicional } from '../info-adicional/info-adicional';
import { FirmaDigitalizada } from '../firma-digitalizada/firma-digitalizada';
import { PostulanteFichaService } from '../../../services/postulante-ficha.service';

@Component({
  selector: 'app-ficha-resumen',
  standalone: true,
  imports: [CommonModule, NgComponentOutlet, MatTabsModule, MatProgressBarModule, MatIconModule, MatProgressSpinnerModule],
  templateUrl: './ficha-resumen.html',
  styleUrls: ['./ficha-resumen.css']
})
export class FichaResumen implements OnInit {
  private resumenService = inject(PostulanteResumenService);
  private fichaService = inject(PostulanteFichaService);
  private alertService = inject(AlertService);

  public cargando = signal<boolean>(false);
  public porcentaje = signal<number>(0);
  public flagsBD = signal<any>({});
  
  // Matriz de control de accesos por pestaña
  public tabsConfigurar: TabConfig[] = [
    { 
      id: 'formacion', 
      titulo: 'Formación Académica', 
      icono: 'school', 
      componente: Formacion, 
      verificarFlag: (f) => f.tieneFormacion,
      inputsComponente: { modoLectura: true } 
    },
    { 
      id: 'colegiatura', 
      titulo: 'Colegiatura', 
      icono: 'card_membership', 
      componente: Colegiatura, 
      verificarFlag: (f) => f.tieneColegiatura,
      inputsComponente: { modoLectura: true }
    },
    { 
      id: 'idiomas', 
      titulo: 'Idiomas', 
      icono: 'translate', 
      componente: Idiomas, 
      verificarFlag: (f) => f.tieneIdiomas,
      inputsComponente: { modoLectura: true }
    },
    { 
      id: 'ofimatica', 
      titulo: 'Ofimática', 
      icono: 'computer', 
      componente: Ofimatica, 
      verificarFlag: (f) => f.tieneOfimatica,
      inputsComponente: { modoLectura: true }
    },
    { 
      id: 'certificaciones', 
      titulo: 'Certificaciones', 
      icono: 'verified', 
      componente: Certificacion, 
      verificarFlag: (f) => f.tieneCertificacion,
      inputsComponente: { modoLectura: true }
    },
    { 
      id: 'experiencia-laboral', 
      titulo: 'Experiencia Laboral', 
      icono: 'work', 
      componente: Experiencia, 
      verificarFlag: (f) => f.tieneExperiencia,
      inputsComponente: { modoLectura: true }
    },
    { 
      id: 'otros-requisitos', 
      titulo: 'Otros Requisitos', 
      icono: 'check_circle', 
      componente: OtrosRequisitos, 
      verificarFlag: (f) => f.tieneOtrosRequisitos,
      inputsComponente: { modoLectura: true }
    },
    { 
      id: 'info-adicional', 
      titulo: 'Información Adicional', 
      icono: 'info', 
      componente: InformacionAdicional, 
      verificarFlag: (f) => f.tieneInformacionAdicional,
      inputsComponente: { modoLectura: true }
    },
    { 
      id: 'firma-digital', 
      titulo: 'Firma Digital', 
      icono: 'fingerprint', 
      componente: FirmaDigitalizada, 
      verificarFlag: (f) => f.tieneFirma,
      inputsComponente: { modoLectura: true }
    }
  ];

  private idPostulante!: number;

  ngOnInit(): void {
    const profile = JSON.parse(sessionStorage.getItem('user_profile') || '{}');
    const tokenParts = atob(profile.token).split('-');
    this.idPostulante = Number(tokenParts[1]);
    this.obtenerDiagnostico();
  }

  obtenerDiagnostico(): void {
    this.cargando.set(true);
    this.resumenService.getAvanceCurriculum(this.idPostulante).subscribe({
      next: (res) => {
        if (res.success) {
          this.porcentaje.set(res.data.porcentajeTotal);
          this.flagsBD.set(res.data);
        }
        this.cargando.set(false);
      },
      error: () => this.cargando.set(false)
    });
  }

  imprimirFichaPDF(): void {
    this.cargando.set(true);

    this.fichaService.imprimirFichaReporte(this.idPostulante).subscribe({
      next: (blob: Blob) => {
        // Creamos un Blob explícito forzando el tipo MIME de PDF
        const pdfBlob = new Blob([blob], { type: 'application/pdf' });
        const fileURL = URL.createObjectURL(pdfBlob);
        
        // 🚀 SOLUCIÓN AL NOMBRE: Creamos un ancla temporal en el DOM
        const link = document.createElement('a');
        link.href = fileURL;
        link.target = '_blank';
        
        // Forzamos el nombre exacto que quieres ver al descargar
        link.download = `FICHA_POSTULANTE_${this.idPostulante.toString().padStart(6, '0')}.pdf`;
        
        // Lo añadimos al cuerpo del DOM, lo gatillamos y lo removemos instantáneamente
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);

        // Limpieza preventiva de memoria
        setTimeout(() => URL.revokeObjectURL(fileURL), 10000);
        this.cargando.set(false);
      },
      error: () => {
        this.cargando.set(false);
        this.alertService.error('Error', 'No se pudo generar el reporte.');
      }
    });
  }
}