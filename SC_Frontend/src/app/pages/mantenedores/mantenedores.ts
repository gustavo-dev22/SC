import { Component, effect, inject, signal } from '@angular/core';
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

@Component({
  selector: 'app-mantenedores',
  imports: [CommonModule, MatTableModule, MatFormFieldModule, MatSelectModule, MatCardModule, MatButtonModule, MatIconModule],
  templateUrl: './mantenedores.html',
  styleUrl: './mantenedores.css',
})
export class Mantenedores {
  private catalogoService = inject(CatalogoService);
  private dialog = inject(MatDialog);

  // Catálogos disponibles fijados para el combo principal
  public catalogosDisponibles = signal([
    { id: 1, nombre: 'Tipos de Documento' },
    { id: 2, nombre: 'Grados Académicos' }
  ]);

  public idCatalogoSeleccionado = signal<number | null>(null);
  public tablaColumnas: string[] = ['id', 'codigo', 'descripcion', 'orden', 'estado', 'acciones'];
  public registrosTabla = signal<any[]>([]);

  constructor() {
    effect(() => {
      const idTipo = this.idCatalogoSeleccionado();
      if (idTipo) {
        this.cargarDatos(idTipo);
      }
    });
  }

  cargarDatos(idTipo: number): void {
    this.catalogoService.getValoresByTipo(idTipo).subscribe({
      next: (res) => {
        if (res.success) {
          this.registrosTabla.set(res.data);
        }
      }
    });
  }

  cambiarSeleccion(idTipo: number): void {
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
        this.catalogoService.mantenimiento(payload).subscribe(res => {
          if (res.success) {
            Swal.fire('Éxito', res.message, 'success');
            this.cargarDatos(idTipoActual); // Recarga la tabla de inmediato
          }
        });
      }
    });
  }

  eliminarLogico(elemento: any): void {
    Swal.fire({
      title: '¿Desactivar Registro?',
      text: `¿Está seguro de ocultar "${elemento.descripcion}" de las convocatorias?`,
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#d33',
      confirmButtonText: 'Sí, Desactivar',
      heightAuto: false
    }).then((result) => {
      if (result.isConfirmed) {
        const payload = {
          accion: 'ELIMINAR_LOGICA',
          idValor: elemento.idValor,
          idTipo: this.idCatalogoSeleccionado(),
          codigoValor: elemento.codigoValor,
          descripcion: elemento.descripcion,
          orden: elemento.orden,
          activo: false
        };

        this.catalogoService.mantenimiento(payload).subscribe(res => {
          if (res.success) {
            Swal.fire('Desactivado', 'El registro ha sido retirado.', 'success');
            this.cargarDatos(this.idCatalogoSeleccionado()!);
          }
        });
      }
    });
  }
}
