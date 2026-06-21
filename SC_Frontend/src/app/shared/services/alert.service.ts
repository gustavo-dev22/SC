import { Injectable, inject } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ModalAlertaComponent } from '../components/modal-alerta/modal-alerta';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class AlertService {
  private dialog = inject(MatDialog);

  // Alerta de Advertencia
  public advertencia(titulo: string, mensaje: string): void {
    this.dialog.open(ModalAlertaComponent, {
      width: '380px',
      disableClose: true,
      autoFocus: 'first-tabbable',
      panelClass: 'clean-alert-window-panel',
      data: { titulo, mensaje, icono: 'warning', tipo: 'advertencia' }
    });
  }

  // Alerta de Errores
  public error(titulo: string, mensaje: string): void {
    this.dialog.open(ModalAlertaComponent, {
      width: '380px',
      disableClose: true,
      autoFocus: 'first-tabbable',
      data: { titulo, mensaje, icono: 'error_outline', tipo: 'error' }
    });
  }

  // Alerta de Éxito 
  public exito(titulo: string, mensaje: string): void {
    this.dialog.open(ModalAlertaComponent, {
      width: '380px',
      disableClose: true,
      autoFocus: 'first-tabbable',
      data: { titulo, mensaje, icono: 'check_circle_outline', tipo: 'exito' }
    });
  }

  public confirmacion(titulo: string, mensaje: string, textoBotonOk: string = 'Sí, Eliminar', textoBotonCancelar: string = 'Cancelar'): Observable<boolean> {
    const dialogRef = this.dialog.open(ModalAlertaComponent, {
      width: '380px',
      disableClose: true,
      autoFocus: 'first-tabbable',
      data: { 
        titulo, 
        mensaje, 
        icono: 'help_outline', 
        tipo: 'confirmacion', 
        esDecision: true,
        textoBotonOk,
        textoBotonCancelar      
      }
    });

    return dialogRef.afterClosed(); 
  }
}