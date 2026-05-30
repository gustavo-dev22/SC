import { Component, effect, inject, OnInit, signal } from '@angular/core';
import { CatalogoService } from '../../services/catalogo.service';
import { MatTableModule } from '@angular/material/table';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { CommonModule } from '@angular/common';
import { MatDialog } from '@angular/material/dialog';
import { ModalForm } from './modal-form/modal-form';
import Swal from 'sweetalert2';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { ModalTipo } from './modal-tipo/modal-tipo';
import { MatMenuModule } from '@angular/material/menu';

@Component({
  selector: 'app-mantenedores',
  imports: [CommonModule, MatTableModule, MatFormFieldModule, MatSelectModule, MatCardModule, MatButtonModule, MatIconModule, MatProgressSpinnerModule, MatPaginatorModule, MatTooltipModule, MatMenuModule],
  templateUrl: './mantenedores.html',
  styleUrl: './mantenedores.css',
})
export class Mantenedores implements OnInit {
  private catalogoService = inject(CatalogoService);
  private dialog = inject(MatDialog);

  // Signals de Control de Estados Estructurados
  public catalogosDisponibles = signal<any[]>([]);
  public idCatalogoSeleccionado = signal<number | null>(null);
  public registrosTabla = signal<any[]>([]);

  // Paginación de Base de Datos (Corregida la letra ñ por n)
  public totalRegistros = signal<number>(0);
  public paginaActual = signal<number>(1);
  public tamanoPagina = signal<number>(10);

  // Estados de Carga Visual
  public cargandoPantallaCompleta = signal<boolean>(false);
  public cargandoTablaSkeleton = signal<boolean>(false);
  
  public tablaColumnas: string[] = ['codigo', 'descripcion', 'orden', 'estado', 'acciones'];
  public dummySkeletonRows = Array(5).fill(0);

  constructor() {
    effect(() => {
      const idTipo = this.idCatalogoSeleccionado();
      if (idTipo) {
        this.cargarDatos(); 
      }
    });
  }

  ngOnInit(): void {
    this.cargarTiposDeCatalogo();
  }

  cargarTiposDeCatalogo(): void {
    this.cargandoPantallaCompleta.set(true);
    this.catalogoService.getTipos().subscribe({
      next: (res) => {
        if (res.success) {
          this.catalogosDisponibles.set(res.data.map((item: any) => ({
            id: item.idTipo,
            codigo: item.codigo,
            nombre: item.nombre
          })));
        }
        this.cargandoPantallaCompleta.set(false);
      },
      error: () => this.cargandoPantallaCompleta.set(false)
    });
  }

  onPageChange(event: PageEvent): void {
    this.paginaActual.set(event.pageIndex + 1);
    this.tamanoPagina.set(event.pageSize);
    this.cargarDatos();
  }

  cargarDatos(): void {
    const idTipo = this.idCatalogoSeleccionado();
    if (!idTipo) return;

    this.cargandoTablaSkeleton.set(true);
    
    this.catalogoService.getValoresByTipo(idTipo, this.paginaActual(), this.tamanoPagina()).subscribe({
      next: (res) => {
        if (res.success) {
          this.registrosTabla.set(res.data);
          this.totalRegistros.set(res.data[0]?.totalRegistros || 0);
        } else {
          this.registrosTabla.set([]);
          this.totalRegistros.set(0);
        }
        this.cargandoTablaSkeleton.set(false);
      },
      error: () => {
        this.registrosTabla.set([]);
        this.totalRegistros.set(0);
        this.cargandoTablaSkeleton.set(false);
      }
    });
  }

  cambiarSeleccion(idTipo: number): void {
    this.paginaActual.set(1);
    this.idCatalogoSeleccionado.set(idTipo);
  }

  abrirFormularioModal(elemento: any = null): void {
    const idTipoActual = this.idCatalogoSeleccionado();
    if (!idTipoActual) return;

    const dialogRef = this.dialog.open(ModalForm, {
      width: '400px',
      data: { idTipo: idTipoActual, elemento: elemento }
    });

    dialogRef.afterClosed().subscribe(payload => {
      if (payload) {
        this.catalogoService.mantenimiento(payload).subscribe({
          next: (res) => {
            if (res.success) {
              Swal.fire('¡Éxito!', res.message, 'success');
              this.cargarDatos();
            } else {
              Swal.fire('Atención', res.message, 'warning');
            }
          },
          error: (err) => {
            const mensajeError = err.error?.message || 'No se pudo procesar el mantenimiento.';
            Swal.fire({
              title: 'Validación de Negocio',
              text: mensajeError,
              icon: 'error',
              confirmButtonText: 'Entendido',
              confirmButtonColor: '#1e3c72',
              heightAuto: false
            });
          }
        });
      }
    });
  }

  cambiarEstadoLogico(elemento: any, nuevoEstado: boolean): void {
    const titulo = nuevoEstado ? '¿Activar Registro?' : '¿Desactivar Registro?';
    const texto = nuevoEstado 
      ? `¿Está seguro de volver a habilitar "${elemento.descripcion}"?`
      : `¿Está seguro de ocultar "${elemento.descripcion}" de las convocatorias?`;
    const btnTexto = nuevoEstado ? 'Sí, Activar' : 'Sí, Desactivar';
    const btnColor = nuevoEstado ? '#10b981' : '#dc2626';

    Swal.fire({
      title: titulo,
      text: texto,
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: btnColor,
      cancelButtonColor: '#64748b',
      confirmButtonText: btnTexto,
      cancelButtonText: 'Cancelar',
      heightAuto: false
    }).then((result) => {
      if (result.isConfirmed) {
        const payload = {
          // Si vamos a activar, usamos 'MODIFICAR' pasándole activo: true. 
          // Si vamos a desactivar, usamos tu acción 'ELIMINAR_LOGICA'
          accion: nuevoEstado ? 'MODIFICAR' : 'ELIMINAR_LOGICA',
          idValor: elemento.idValor,
          idTipo: this.idCatalogoSeleccionado(),
          codigoValor: elemento.codigoValor,
          descripcion: elemento.descripcion,
          orden: elemento.orden,
          activo: nuevoEstado // Aquí viaja true o false de forma explícita
        };

        this.catalogoService.mantenimiento(payload).subscribe({
          next: (res) => {
            if (res.success) {
              const msgExito = nuevoEstado ? 'El registro ha sido habilitado.' : 'El registro ha sido retirado.';
              Swal.fire('¡Operación Exitosa!', msgExito, 'success');
              this.cargarDatos(); // Recarga la tabla de inmediato
            }
          },
          error: (err) => {
            const msg = err.error?.message || 'No se pudo cambiar el estado del registro.';
            Swal.fire('Error', msg, 'error');
          }
        });
      }
    });
  }

  abrirModalNuevoCatalogoRaiz(): void {
    const dialogRef = this.dialog.open(ModalTipo, { width: '400px' });

    dialogRef.afterClosed().subscribe(payload => {
      if (payload) {
        // Forzamos el código a mayúsculas limpias antes de enviar
        payload.codigo = payload.codigo.toUpperCase().trim();

        this.catalogoService.mantenimientoTipo(payload).subscribe({
          next: (res) => {
            if (res.success) {
              Swal.fire('¡Eje Creado!', res.message, 'success');
              
              // MAGIA SENIOR: Volvemos a consultar al backend. Al actualizar el Signal, 
              // el mat-select del HTML se refresca automáticamente con el nuevo catálogo en la lista.
              this.cargarTiposDeCatalogo(); 
            }
          },
          error: (err) => {
            const msg = err.error?.message || 'Error al crear el catálogo raíz.';
            Swal.fire('Error de Duplicidad', msg, 'error');
          }
        });
      }
    });
  }

  abrirModalEditarCatalogoRaiz(): void {
    const idTipoActual = this.idCatalogoSeleccionado();
    const catalogoActual = this.catalogosDisponibles().find(c => c.id === idTipoActual);
    
    if (!catalogoActual) return;

    const dialogRef = this.dialog.open(ModalTipo, {
      width: '400px',
      data: { 
        elemento: { 
          idTipo: catalogoActual.id, 
          codigo: catalogoActual.codigo,
          nombre: catalogoActual.nombre, 
          activo: true 
        } 
      }
    });

    dialogRef.afterClosed().subscribe(payload => {
      if (payload) {
        this.catalogoService.mantenimientoTipo(payload).subscribe({
          next: (res) => {
            if (res.success) {
              Swal.fire('¡Actualizado!', 'El nombre del catálogo se modificó correctamente.', 'success');
              this.cargarTiposDeCatalogo(); // Refresca la lista del combo inmediatamente
            }
          }
        });
      }
    });
  }

  eliminarCatalogoRaizLogico(): void {
    const idTipoActual = this.idCatalogoSeleccionado();
    const catalogoActual = this.catalogosDisponibles().find(c => c.id === idTipoActual);
    if (!catalogoActual) return;

    Swal.fire({
      title: '¿Desactivar Catálogo Completo?',
      text: `Esto ocultará el catálogo "${catalogoActual.nombre}" y todos sus sub-registros del sistema de convocatorias.`,
      icon: 'error',
      showCancelButton: true,
      confirmButtonColor: '#dc2626',
      confirmButtonText: 'Sí, Desactivar Todo',
      cancelButtonText: 'Cancelar',
      heightAuto: false
    }).then((result) => {
      if (result.isConfirmed) {
        const payload = {
          accion: 'ELIMINAR_LOGICA',
          idTipo: catalogoActual.id,
          codigo: '',
          nombre: catalogoActual.nombre,
          activo: false
        };

        this.catalogoService.mantenimientoTipo(payload).subscribe({
          next: (res) => {
            if (res.success) {
              Swal.fire('Catálogo Retirado', 'El catálogo ha sido dado de baja.', 'success');
              this.idCatalogoSeleccionado.set(null); // Limpiamos la selección actual
              this.registrosTabla.set([]); // Limpiamos la tabla
              this.cargarTiposDeCatalogo(); // Refrescamos el combo
            }
          }
        });
      }
    });
  }
}
