import { Component, inject, OnInit, signal } from '@angular/core';
import { AuditoriaService } from '../../../services/auditoria.service';
import { CommonModule } from '@angular/common';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTableModule } from '@angular/material/table';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule, MAT_DATE_LOCALE, DateAdapter } from '@angular/material/core';

@Component({
  selector: 'app-logs-auditoria',
  imports: [CommonModule, MatProgressSpinnerModule, MatTableModule, MatIconModule, MatFormFieldModule, MatSelectModule, MatInputModule, MatDatepickerModule, MatNativeDateModule],
  templateUrl: './logs-auditoria.html',
  styleUrl: './logs-auditoria.css',
})
export class LogsAuditoria implements OnInit {
  private audService = inject(AuditoriaService);
  private dateAdapter = inject(DateAdapter<Date>);

  public logs = signal<any[]>([]);
  public columnas = ['id', 'operacion', 'fecha', 'usuario', 'data'];
  public cargando = signal<boolean>(false);

  public filtroOperacion = signal<string | null>(null);
  public filtroFechaInicio = signal<string | null>(null);
  public filtroFechaFin = signal<string | null>(null);

  ngOnInit(): void {
    this.dateAdapter.setLocale('es-PE');
    this.cargarLogs();
  }

  cargarLogs(): void {
    this.cargando.set(true);
    
    this.audService.listarLogs(
      this.filtroOperacion() ?? undefined,
      this.filtroFechaInicio() ?? undefined,
      this.filtroFechaFin() ?? undefined
    ).subscribe({
      next: (res) => {
        if (res.success) this.logs.set(res.data);
        this.cargando.set(false);
      },
      error: () => this.cargando.set(false)
    });
  }

  filtrarPorOperacion(operacion: string | null): void {
    this.filtroOperacion.set(operacion);
    this.cargarLogs();
  }

  cambiarFechaInicio(fecha: Date | null): void {
    if (!fecha) {
      this.filtroFechaInicio.set(null);
    } else {
      const offset = fecha.getTimezoneOffset();
      const fechaLocal = new Date(fecha.getTime() - (offset * 60 * 1000));
      this.filtroFechaInicio.set(fechaLocal.toISOString().split('T')[0]);
    }
    this.cargarLogs();
  }

  cambiarFechaFin(fecha: Date | null): void {
    if (!fecha) {
      this.filtroFechaFin.set(null);
    } else {
      const fechaConHoraMaxima = new Date(fecha);
      fechaConHoraMaxima.setHours(23, 59, 59, 999);
      
      const offset = fechaConHoraMaxima.getTimezoneOffset();
      const fechaLocal = new Date(fechaConHoraMaxima.getTime() - (offset * 60 * 1000));
      
      this.filtroFechaFin.set(fechaLocal.toISOString());
    }
    this.cargarLogs();
  }
}
