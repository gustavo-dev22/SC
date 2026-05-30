import { CommonModule } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { ParametroService } from '../../services/parametro.service';
import Swal from 'sweetalert2';
import { MatDialog } from '@angular/material/dialog';
import { ModalParametro } from './modal-parametro/modal-parametro';

@Component({
  selector: 'app-parametros',
  imports: [CommonModule, FormsModule, MatCardModule, MatFormFieldModule, MatInputModule, MatButtonModule, MatIconModule, MatSlideToggleModule, MatProgressSpinnerModule],
  templateUrl: './parametros.html',
  styleUrl: './parametros.css',
})
export class Parametros implements OnInit {
  private parametroService = inject(ParametroService);
  private dialog = inject(MatDialog);

  public parametrosOriginales = signal<any[]>([]);
  public cargando = signal<boolean>(false);

  ngOnInit(): void {
    this.cargarParametros();
  }

  cargarParametros(): void {
    this.cargando.set(true);
    this.parametroService.getParametros().subscribe({
      next: (res) => {
        if (res.success) {
          this.parametrosOriginales.set(res.data);
        }
        this.cargando.set(false);
      },
      error: () => this.cargando.set(false)
    });
  }

  guardarCambioRapido(codigo: string, valor: string): void {
    this.cargando.set(true);
    const payload = { accion: 'MODIFICAR', codigo, valor, nombre: 'N/A', descripcion: 'N/A', categoria: 'N/A' };
    this.parametroService.updateParametro(payload).subscribe({
      next: () => this.cargarParametros(),
      error: () => this.cargando.set(false)
    });
  }

  abrirModal(elemento: any = null): void {
    const dialogRef = this.dialog.open(ModalParametro, { width: '420px', data: { elemento } });

    dialogRef.afterClosed().subscribe(payload => {
      if (payload) {
        this.cargando.set(true);
        this.parametroService.updateParametro(payload).subscribe({
          next: (res) => {
            if (res.success) {
              Swal.fire('Éxito', res.message, 'success');
              this.cargarParametros();
            }
          },
          error: (err) => {
            const msg = err.error?.message || 'Error en validación.';
            Swal.fire('Atención', msg, 'error');
            this.cargando.set(false);
          }
        });
      }
    });
  }

  guardarCambio(codigo: string, nuevoValor: string): void {
    this.cargando.set(true);
    
    // Si el valor es un booleano proveniente de un SlideToggle, lo pasamos a "1" o "0" para SQL Server
    const valorFormateado = nuevoValor.toString() === 'true' ? '1' : 
                            nuevoValor.toString() === 'false' ? '0' : nuevoValor.trim();

    this.parametroService.updateParametro({ codigo, valor: valorFormateado }).subscribe({
      next: (res) => {
        if (res.success) {
          Swal.fire({
            title: '¡Guardado!',
            text: 'La regla operativa del sistema se actualizó inmediatamente.',
            icon: 'success',
            timer: 2000,
            showConfirmButton: false
          });
          this.cargarParametros(); // Refresca los estados locales
        }
      },
      error: () => this.cargando.set(false)
    });
  }

  eliminarParametro(param: any): void {
    Swal.fire({
      title: '¿Eliminar Parámetro?',
      text: `¿Está seguro de borrar la llave "${param.codigo}"? Esto puede afectar el código duro del sistema.`,
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#dc2626',
      confirmButtonText: 'Sí, Eliminar'
    }).then((result) => {
      if (result.isConfirmed) {
        this.cargando.set(true);
        const payload = { accion: 'ELIMINAR', codigo: param.codigo, nombre: 'N/A', valor: 'N/A', descripcion: 'N/A', categoria: 'N/A' };
        this.parametroService.updateParametro(payload).subscribe({
          next: () => {
            Swal.fire('Eliminado', 'La regla operativa fue purgada.', 'success');
            this.cargarParametros();
          },
          error: () => this.cargando.set(false)
        });
      }
    });
  }
}
