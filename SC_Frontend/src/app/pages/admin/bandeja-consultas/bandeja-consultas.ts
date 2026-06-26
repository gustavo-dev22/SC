import { Component, inject, OnDestroy, OnInit, signal } from '@angular/core';
import { AdminSoporteService } from '../../../services/admin-soporte.service';
import { AlertService } from '../../../shared/services/alert.service';
import { CommonModule } from '@angular/common';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTableModule } from '@angular/material/table';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { CatalogoService } from '../../../services/catalogo.service';
import { debounceTime, distinctUntilChanged, Subject } from 'rxjs';

@Component({
  selector: 'app-bandeja-consultas',
  imports: [CommonModule, MatProgressSpinnerModule, MatTableModule, MatInputModule, MatButtonModule, MatFormFieldModule, MatIconModule, MatSelectModule],
  templateUrl: './bandeja-consultas.html',
  styleUrl: './bandeja-consultas.css',
})
export class BandejaConsultas implements OnInit, OnDestroy {
  private adminService = inject(AdminSoporteService);
  private catalogoService = inject(CatalogoService);
  private alertService = inject(AlertService);

  public ticketsBandeja = signal<any[]>([]);
  public listaEstadosTicket = signal<any[]>([]);
  public cargando = signal<boolean>(false);
  private nombreAdmin!: string;

  public filtroEstado = signal<number | undefined>(undefined);
  public filtroTexto = signal<string>('');

  private buscadorSubject = new Subject<string>();

  ngOnInit(): void {
    // Jalamos los datos del usuario logueado en el SASI
    const profile = JSON.parse(sessionStorage.getItem('user_profile') || '{}');
    this.nombreAdmin = profile.nombreUsuario || 'Soporte OTI';
    
    this.cargarEstadosCatalogo();
    this.cargarBandejaGlobal();

    this.buscadorSubject.pipe(
      debounceTime(700),
      distinctUntilChanged()
    ).subscribe(texto => {
      this.filtroTexto.set(texto);
      this.cargarBandejaGlobal();
    });
  }

  ngOnDestroy(): void {
    this.buscadorSubject.complete();
  }

  cargarEstadosCatalogo(): void {
    this.catalogoService.getValoresByCodigo('ESTADO_TICKET_ATENCION').subscribe({
      next: (res) => {
        if (res.success) {
          this.listaEstadosTicket.set(res.data);
        }
      }
    });
  }

  cargarBandejaGlobal(): void {
    this.cargando.set(true);

    this.adminService.obtenerBandeja(this.filtroEstado(), this.filtroTexto()).subscribe({
      next: (res) => {
        if (res.success) this.ticketsBandeja.set(res.data);
        this.cargando.set(false);
      },
      error: () => this.cargando.set(false)
    });
  }

  procesarTicket(idTicket: number, respuesta: string, nuevoEstado: number): void {
    if ((nuevoEstado === 1105 || nuevoEstado === 1106) && !respuesta?.trim()) {
      this.alertService.error('Respuesta Requerida', 'Debe redactar una solución o sustento antes de guardar.');
      return;
    }

    this.cargando.set(true);
    const command = {
      idTicket: idTicket,
      respuestaSoporte: respuesta?.trim() || null, 
      idEstado: nuevoEstado,
      nombreAdmin: this.nombreAdmin
    };

    this.adminService.atenderTicket(command).subscribe({
      next: (res) => {
        this.alertService.exito('Procesado', 'El ticket fue actualizado y se le notificó al postulante.');
        this.cargarBandejaGlobal(); // Recargamos la grilla en caliente
      },
      error: (err) => {
        this.cargando.set(false);
        this.alertService.error('Error', err.error?.message || 'No se pudo procesar la acción.');
      }
    });
  }

  aplicarFiltroEstado(estado?: number): void {
    this.filtroEstado.set(estado);
    this.cargarBandejaGlobal();
  }

  onKeyupBusqueda(event: Event): void {
    const valor = (event.target as HTMLInputElement).value;
    this.buscadorSubject.next(valor);
  }
}
