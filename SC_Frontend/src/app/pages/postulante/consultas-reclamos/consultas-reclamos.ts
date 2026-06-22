import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { SoporteService } from '../../../services/soporte.service';
import { AlertService } from '../../../shared/services/alert.service';
import { CommonModule } from '@angular/common';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { CatalogoService } from '../../../services/catalogo.service';
import { OportunidadesService } from '../../../services/oportunidades.service';

@Component({
  selector: 'app-consultas-reclamos',
  imports: [CommonModule, MatProgressSpinnerModule, MatCardModule, MatFormFieldModule, MatInputModule, MatSelectModule, ReactiveFormsModule, FormsModule, MatIconModule, MatButtonModule],
  templateUrl: './consultas-reclamos.html',
  styleUrl: './consultas-reclamos.css',
})
export class ConsultasReclamos implements OnInit {
  private ss = inject(SoporteService);
  private alertService = inject(AlertService);
  private catalogoService = inject(CatalogoService);
  private os = inject(OportunidadesService);

  public listaTickets = signal<any[]>([]);
  public listaTiposTicket = signal<any[]>([]);
  public listaMisPostulaciones = signal<any[]>([]);
  public cargando = signal<boolean>(false);

  // Inputs del formulario
  public tipoSeleccionado = signal<number | undefined>(undefined);
  public plazaSeleccionada = signal<number | null>(null);
  public asuntoText = '';
  public descripcionText = '';
  private idPostulante!: number;

  public esReclamoCalificacion = computed(() => this.tipoSeleccionado() === 1101);

  ngOnInit(): void {
    const profile = JSON.parse(sessionStorage.getItem('user_profile') || '{}');
    const tokenParts = atob(profile.token).split('-');
    this.idPostulante = Number(tokenParts[1]);

    this.cargarCatalogos();
    this.cargarHistorialTickets();
    this.cargarMisPostulacionesConformes();
  }

  cargarCatalogos(): void {
    this.catalogoService.getValoresByCodigo('TIPO_TICKET').subscribe({
      next: (res) => {
        if (res.success) {
          this.listaTiposTicket.set(res.data);
          this.tipoSeleccionado.set(res.data[0].idValor);
        }
      }
    });
  }

  cargarMisPostulacionesConformes(): void {
    this.os.obtenerMisPostulaciones(this.idPostulante).subscribe({
      next: (res) => {
        if (res.success) {
          this.listaMisPostulaciones.set(res.data); // Mapea idPlaza y CodigoConvocatoria/NombrePuesto
        }
      }
    });
  }

  cargarHistorialTickets(): void {
    this.cargando.set(true);
    this.ss.listarTickets(this.idPostulante).subscribe({
      next: (res) => {
        console.log(res);
        if (res.success) this.listaTickets.set(res.data);
        this.cargando.set(false);
      },
      error: () => this.cargando.set(false)
    });
  }

  guardarTicket(): void {
    if (this.esReclamoCalificacion() && !this.plazaSeleccionada()) {
      this.alertService.error('Plaza Requerida', 'Para interponer un reclamo debe seleccionar la convocatoria asociada.');
      return;
    }

    if (!this.tipoSeleccionado || !this.asuntoText.trim() || !this.descripcionText.trim()) {
      this.alertService.error('Campos Incompletos', 'Por favor, complete todos los campos del formulario.');
      return;
    }

    this.cargando.set(true);
    const command = {
      idPostulante: this.idPostulante,
      idPlaza: this.esReclamoCalificacion() ? this.plazaSeleccionada() : null,
      idTipoTicketCat: this.tipoSeleccionado(),
      asunto: this.asuntoText,
      descripcion: this.descripcionText
    };

    this.ss.enviarTicket(command).subscribe({
      next: (res) => {
        this.alertService.exito('Enviado', 'Su solicitud fue registrada. Recursos Humanos le responderá a la brevedad.');
        this.asuntoText = '';
        this.descripcionText = '';
        this.plazaSeleccionada.set(null);
        this.cargarHistorialTickets(); // Refrescamos el listado
      },
      error: (err) => {
        this.cargando.set(false);
        this.alertService.error('Error', err.error?.message || 'No se pudo enviar su ticket.');
      }
    });
  }
}
