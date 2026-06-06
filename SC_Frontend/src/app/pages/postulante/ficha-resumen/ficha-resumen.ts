import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule, NgComponentOutlet } from '@angular/common';
import { MatTabsModule } from '@angular/material/tabs';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { PostulanteResumenService } from '../../../services/postulante-resumen.service';
import { TabConfig } from '../../../core/models/tabConfig.model';
import { AlertService } from '../../../shared/services/alert.service';

// Importamos los subcomponentes de las listas para meterlos en los TABS
import { Formacion } from '../formacion/formacion';
import { Colegiatura } from '../colegiatura/colegiatura';
import { Idiomas } from '../idioma/idioma';
import { Ofimatica } from '../ofimatica/ofimatica';
import { Certificacion } from '../certificacion/certificacion';
import { Experiencia } from '../experiencia/experiencia';
import { OtrosRequisitos } from '../otros-requisitos/otros-requisitos';

@Component({
  selector: 'app-ficha-resumen',
  standalone: true,
  imports: [CommonModule, NgComponentOutlet, MatTabsModule, MatProgressBarModule, MatIconModule, MatProgressSpinnerModule],
  templateUrl: './ficha-resumen.html',
  styleUrls: ['./ficha-resumen.css']
})
export class FichaResumen implements OnInit {
  private resumenService = inject(PostulanteResumenService);
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
    this.alertService.advertencia(
      'Módulo en Desarrollo',
      'La generación del reporte automatizado en formato PDF se encuentra en fase de maquetación con iTextSharp / QuestPDF en el Backend. Estará disponible en la siguiente entrega.'
    );
  }
}