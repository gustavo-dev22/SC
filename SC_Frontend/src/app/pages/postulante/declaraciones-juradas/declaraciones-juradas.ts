import { Component, computed, inject, Input, OnInit, signal } from '@angular/core';
import { PostulanteDeclaracionService } from '../../../services/postulante-declaracion.service';
import { AlertService } from '../../../shared/services/alert.service';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatCardModule } from '@angular/material/card';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';
import { CatalogoService } from '../../../services/catalogo.service';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-declaraciones-juradas',
  imports: [CommonModule, MatButtonModule, MatProgressSpinnerModule, MatCardModule, MatCheckboxModule, ReactiveFormsModule, FormsModule, MatIconModule],
  templateUrl: './declaraciones-juradas.html',
  styleUrl: './declaraciones-juradas.css',
})
export class DeclaracionesJuradas implements OnInit {
  private ds = inject(PostulanteDeclaracionService);
  private catalogoService = inject(CatalogoService);
  private alertService = inject(AlertService);
  private authService = inject(AuthService);
  
  public listaDeclaraciones = signal<any[]>([]);
  public cargando = signal<boolean>(false);
  @Input() modoLectura: boolean = false;
  private idPostulante!: number;

  // 🚀 SIGNAL COMPUTADA: Evalúa dinámicamente si falta alguna DDJJ por aceptar
  public todasAceptadas = computed(() => {
    const actuales = this.listaDeclaraciones();
    if (actuales.length === 0) return false;
    return actuales.every(d => d.aceptado === true);
  });

  ngOnInit(): void {
    this.cargando.set(true);
    this.idPostulante = this.authService.obtenerIdPostulanteDesdeJwt();
    
    if (this.idPostulante > 0) {
      this.cargarDatosPorCatalogo();
    } else {
      this.alertService.error('Error de Sesión', 'No se pudo identificar al postulante. Por favor reinicie sesión.');
    }
  }

  onCheckChange(idDeclaracionCat: number, nuevoValor: boolean): void {
    this.listaDeclaraciones.update(declaraciones => 
      declaraciones.map(d => 
        d.idDeclaracionCat === idDeclaracionCat 
          ? { ...d, aceptado: nuevoValor } 
          : d
      )
    );
  }

  cargarDatosPorCatalogo(): void {
    this.catalogoService.getValoresByCodigo('DDJJ').subscribe({
      next: (res) => {
        if (res.success && res.data && res.data.length > 0) {
          const idTipoDinamico = res.data[0].idTipo;
          
          this.cargarDeclaracionesPostulante(idTipoDinamico);
        } else {
          this.cargando.set(false);
          this.alertService.error('Error', 'No se pudo inicializar el catálogo de declaraciones juradas.');
        }
      },
      error: () => {
        this.cargando.set(false);
        this.alertService.error('Error', 'Error de red al consumir el catálogo maestro.');
      }
    });
  }

  cargarDeclaracionesPostulante(idTipo: number): void {
    this.ds.getDeclaraciones(this.idPostulante, idTipo).subscribe({
      next: (res) => {
        this.listaDeclaraciones.set(res);
        this.cargando.set(false);
      },
      error: () => {
        this.cargando.set(false);
        this.alertService.error('Error', 'No se pudieron recuperar tus declaraciones registradas.');
      }
    });
  }

  guardarCambios(): void {
    this.cargando.set(true);
    this.ds.guardarDeclaraciones(this.idPostulante, this.listaDeclaraciones()).subscribe({
      next: () => {
        this.cargando.set(false);
        this.alertService.exito('¡Completado!', 'Las declaraciones juradas se guardaron correctamente.');
      },
      error: () => {
        this.cargando.set(false);
        this.alertService.error('Error', 'No se pudieron registrar las conformidades.');
      }
    });
  }
}
