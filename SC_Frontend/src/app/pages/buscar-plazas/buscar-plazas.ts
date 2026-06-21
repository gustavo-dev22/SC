import { Component, inject, OnInit, OnDestroy, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { forkJoin, Subject, Subscription } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { OportunidadesService } from '../../services/oportunidades.service';
import { AlertService } from '../../shared/services/alert.service';
import { DetallePlaza } from './detalle-plaza/detalle-plaza';
import { PostulanteResumenService } from '../../services/postulante-resumen.service';

@Component({
  selector: 'app-buscar-plazas',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatPaginatorModule,
    MatDialogModule
  ],
  templateUrl: './buscar-plazas.html'
})
export class BuscarPlazas implements OnInit, OnDestroy {
  private os = inject(OportunidadesService);
  private alertService = inject(AlertService);
  private resumenService = inject(PostulanteResumenService);
  public dialog = inject(MatDialog);

  public listaPlazas = signal<any[]>([]);
  public cargando = signal<boolean>(false);

  public porcentajeFicha = signal<number>(0);
  
  public totalRegistros = signal<number>(0);
  public paginaActual = signal<number>(1);
  public registrosPorPagina = signal<number>(10);

  private filtroBusqueda: string = '';
  private idPostulante!: number;

  private searchSubject = new Subject<string>();
  private searchSubscription!: Subscription;

  ngOnInit(): void {
    const profile = JSON.parse(sessionStorage.getItem('user_profile') || '{}');
    const tokenParts = atob(profile.token).split('-');
    this.idPostulante = Number(tokenParts[1]);

    this.searchSubscription = this.searchSubject.pipe(
      debounceTime(400),
      distinctUntilChanged()
    ).subscribe(textoBuscar => {
      this.filtroBusqueda = textoBuscar;
      this.paginaActual.set(1);
      this.cargarPlazas();
    });

    this.inicializarModuloPlazas();
  }

  inicializarModuloPlazas(): void {
    this.cargando.set(true);

    // Ejecutamos ambas peticiones en paralelo y controlamos el spinner en un solo bloque reactivo
    forkJoin({
      avance: this.resumenService.getAvanceCurriculum(this.idPostulante),
      plazas: this.os.buscarPlazasVacantes(this.idPostulante, this.filtroBusqueda, this.paginaActual(), this.registrosPorPagina())
    }).subscribe({
      next: (resultado) => {
        // 1. Procesamos el avance del currículum
        if (resultado.avance.success) {
          this.porcentajeFicha.set(resultado.avance.data.porcentajeTotal);
        }
        
        // 2. Procesamos el listado de plazas vacantes
        this.listaPlazas.set(resultado.plazas.content);
        this.totalRegistros.set(resultado.plazas.totalElements);
        
        this.cargando.set(false); // Apagamos el overlay de forma segura
      },
      error: () => {
        this.cargando.set(false);
        this.porcentajeFicha.set(0);
      }
    });
  }

  cargarPlazas(): void {
    this.cargando.set(true);
    this.os.buscarPlazasVacantes(this.idPostulante, this.filtroBusqueda, this.paginaActual(), this.registrosPorPagina()).subscribe({
      next: (res) => {
        this.listaPlazas.set(res.content);
        this.totalRegistros.set(res.totalElements);
        this.cargando.set(false);
      },
      error: () => this.cargando.set(false)
    });
  }

  onCambiarPagina(event: any): void {
    this.paginaActual.set(event.pageIndex + 1);
    this.registrosPorPagina.set(event.pageSize);
    this.cargarPlazas();
  }

  onSearchKeyup(event: any): void {
    const value = event.target.value;
    this.searchSubject.next(value);
  }

  verificarVencimiento(fechaFinStr: string | Date): boolean {
    const hoy = new Date();
    hoy.setHours(0, 0, 0, 0);
    const fechaFin = new Date(fechaFinStr);
    fechaFin.setHours(0, 0, 0, 0);
    return fechaFin < hoy;
  }

  abrirDetailPlaza(plaza: any): void {
    const esVencido = this.verificarVencimiento(plaza.fechaFin);

    // 🚀 CORREGIDO: Usamos el método .open() nativo de MatDialog
    const dialogRef = this.dialog.open(DetallePlaza, {
      width: '550px',
      disableClose: false,
      data: { plaza, esVencida: esVencido }
    });

    dialogRef.afterClosed().subscribe((procederPostulacion: boolean) => {
      if (procederPostulacion) {
        this.validarRequisitosPreviosPostulacion(plaza);
      }
    });
  }

  validarRequisitosPreviosPostulacion(plaza: any): void {
    // Evaluamos el valor de la Signal en memoria
    if (this.porcentajeFicha() < 100) {
      
      // Caso < 100%: Alerta restrictiva/informativa usando tu modal dinámico
      this.alertService.confirmacion(
        'Ficha Incompleta',
        `Para postular a la plaza de "${plaza.nombrePuesto}" requiere tener el 100% de su Currículum / Ficha de Postulación llenada. Actualmente se encuentra al ${this.porcentajeFicha()}%.`,
        'Entendido',      // Texto botón Ok
        'Cancelar'         // Texto botón Cancelar
      ).subscribe();
      
      return; // Detenemos el flujo inmediatamente
    }

    // Caso == 100%: Abre la confirmación oficial que ya tenías programada
    this.abrirConfirmacionPostulacion(plaza);
  }

  abrirConfirmacionPostulacion(plaza: any): void {
    this.alertService.confirmacion(
      '¿Desea postular?', 
      `¿Está seguro de aplicar al puesto de ${plaza.nombrePuesto}? Una vez enviado, no podrá modificar su ficha resumen.`,
      'Sí, Postular',
      'Regresar'
    ).subscribe((seConfirmo: boolean) => {
      if (seConfirmo) {
        this.ejecutarFlujoPostulacion(plaza);
      }
    });
  }

  private ejecutarFlujoPostulacion(plaza: any): void {
    this.cargando.set(true);

    this.os.registrarPostulacion(this.idPostulante, plaza.idPlaza, plaza.fechaFin, plaza.yaPostulo).subscribe({
      next: (res) => {
        this.cargando.set(false);
        this.alertService.exito('¡Éxito!', 'Su postulación ha sido procesada de manera conforme.');

        // 🚀 MUTACIÓN REACTIVA DE SIGNAL: Buscamos la plaza en la lista y cambiamos 'yaPostulo' a true
        this.listaPlazas.update(plazas => 
          plazas.map(p => p.idPlaza === plaza.idPlaza ? { ...p, yaPostulo: true } : p)
        );
      },
      error: (err) => {
        this.cargando.set(false);
        // Muestra el mensaje exacto que escupió la capa de Dominio
        this.alertService.error('Error', err.error?.message || 'Ocurrió un inconveniente al postular.');
      }
    });
  }

  ngOnDestroy(): void {
    if (this.searchSubscription) this.searchSubscription.unsubscribe();
  }
}