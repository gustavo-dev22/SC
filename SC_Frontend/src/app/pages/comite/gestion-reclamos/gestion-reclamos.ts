import { Component, inject, OnDestroy, OnInit, signal } from '@angular/core';
import { ComiteEvaluacionService } from '../../../services/comite-evaluacion.service';
import { CatalogoService } from '../../../services/catalogo.service';
import { AlertService } from '../../../shared/services/alert.service';
import { debounceTime, distinctUntilChanged, Subject } from 'rxjs';
import { CommonModule } from '@angular/common';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatTableModule } from '@angular/material/table';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

@Component({
  selector: 'app-gestion-reclamos',
  imports: [CommonModule, MatPaginatorModule, MatTableModule, MatFormFieldModule, MatInputModule, MatSelectModule, MatButtonModule, MatIconModule, MatProgressSpinnerModule],
  templateUrl: './gestion-reclamos.html',
  styleUrl: './gestion-reclamos.css',
})
export class GestionReclamos implements OnInit, OnDestroy {
  private comiteService = inject(ComiteEvaluacionService);
  private catalogoService = inject(CatalogoService);
  private alertService = inject(AlertService);

  public ticketsBandeja = signal<any[]>([]);
  public listaEstadosTicket = signal<any[]>([]);
  public cargando = signal<boolean>(false);
  private nombreUsuario!: string;

  public filtroEstado = signal<number | undefined>(undefined);
  public filtroTexto = signal<string>('');

  private buscadorSubject = new Subject<string>();

  ngOnInit(): void {
    const profile = JSON.parse(sessionStorage.getItem('user_profile') || '{}');
    this.nombreUsuario = profile.nombreUsuario || 'Miembro de Comité';
    
    this.cargarEstadosCatalogo();
    this.cargarConsultasTecnicas();

    this.buscadorSubject.pipe(
      debounceTime(500),
      distinctUntilChanged()
    ).subscribe(texto => {
      this.filtroTexto.set(texto);
      this.cargarConsultasTecnicas();
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

  cargarConsultasTecnicas(): void {
    this.cargando.set(true);
    this.comiteService.obtenerConsultasTecnicas(this.filtroEstado(), this.filtroTexto()).subscribe({
      next: (res) => {
        if (res.success) this.ticketsBandeja.set(res.data);
        this.cargando.set(false);
      },
      error: () => this.cargando.set(false)
    });
  }

  procesarTicket(idTicket: number, respuesta: string, nuevoEstado: number): void {
    if ((nuevoEstado === 1105 || nuevoEstado === 1106) && !respuesta?.trim()) {
      this.alertService.advertencia('Respuesta Requerida', 'Debe redactar un sustento o solución técnica antes de proceder.');
      return;
    }

    this.cargando.set(true);
    const command = {
      idTicket: idTicket,
      respuestaSoporte: respuesta?.trim() || null, 
      idEstado: nuevoEstado,
      nombreAdmin: this.nombreUsuario
    };

    this.comiteService.atenderConsultaTecnica(command).subscribe({
      next: (res) => {
        this.cargando.set(false);
        const operacionExitosa = (res && typeof res === 'object') ? res.success : res;

        if (operacionExitosa) {
          this.alertService.exito('Procesado', 'La consulta técnica fue actualizada y se le notificó al postulante.');
          this.cargarConsultasTecnicas(); // Refresco reactivo en caliente
        } else {
          this.alertService.error('Error', 'No se pudo procesar la atención de la consulta.');
        }
      },
      error: (err) => {
        this.cargando.set(false);
        this.alertService.error('Error', err.error?.message || 'Error de comunicación con el servidor central.');
      }
    });
  }

  aplicarFiltroEstado(estado?: number): void {
    this.filtroEstado.set(estado);
    this.cargarConsultasTecnicas();
  }

  onKeyupBusqueda(event: Event): void {
    const valor = (event.target as HTMLInputElement).value;
    this.buscadorSubject.next(valor);
  }
}
