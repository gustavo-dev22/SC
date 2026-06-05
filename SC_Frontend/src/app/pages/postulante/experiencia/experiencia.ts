import { CommonModule } from '@angular/common';
import { Component, computed, inject, OnInit, signal, ViewContainerRef } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { PostulanteExperienciaService } from '../../../services/postulante-experiencia.service';
import Swal from 'sweetalert2';
import { ModalExperiencia } from './modal-experiencia/modal-experiencia';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { AlertService } from '../../../shared/services/alert.service';

@Component({
  selector: 'app-experiencia',
  imports: [CommonModule, MatDialogModule, MatButtonModule, MatIconModule, MatTooltipModule, MatProgressSpinnerModule],
  templateUrl: './experiencia.html',
  styleUrl: './experiencia.css',
})
export class Experiencia implements OnInit {
  private dialog = inject(MatDialog);
  private expService = inject(PostulanteExperienciaService);
  private viewContainerRef = inject(ViewContainerRef);
  private alertService = inject(AlertService);

  public listaExperiencias = signal<any[]>([]);
  public cargando = signal<boolean>(false);
  private idPostulante!: number;

  // 🚀 REGLA DE NEGOCIO ENFOQUE OFICIAL:
  // 1. Experiencia General: Suma TODOS los días registrados en su historial
  public resumenGeneral = computed(() => this.convertirDiasALegible(
    this.listaExperiencias().reduce((acc, item) => acc + item.totalDiasAcumulados, 0)
  ));

  // 2. Experiencia Específica: Suma únicamente los que aplican al perfil del puesto vacante
  public resumenEspecifico = computed(() => this.convertirDiasALegible(
    this.listaExperiencias().filter(x => x.esExperienciaEspecifica).reduce((acc, item) => acc + item.totalDiasAcumulados, 0)
  ));

  ngOnInit(): void {
    const profile = JSON.parse(sessionStorage.getItem('user_profile') || '{}');
    const tokenParts = atob(profile.token).split('-');
    this.idPostulante = Number(tokenParts[1]);
    this.cargarExperiencias();
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

  public convertirDiasALegible(totalDias: number): string {
    if (totalDias <= 0) return '0 días';
    
    const anios = Math.floor(totalDias / 365);
    const diasRestantesAnio = totalDias % 365;
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
