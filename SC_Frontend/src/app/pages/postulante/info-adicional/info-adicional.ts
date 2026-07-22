import { Component, inject, Input, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatRadioModule } from '@angular/material/radio';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { UbigeoService } from '../../../services/ubigeo.service';
import { PostulanteInfoAdicionalService } from '../../../services/postulante-info-adicional.service';
import { AlertService } from '../../../shared/services/alert.service';
import { forkJoin } from 'rxjs';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-informacion-adicional',
  standalone: true,
  imports: [
    CommonModule, 
    ReactiveFormsModule, 
    MatFormFieldModule, 
    MatSelectModule, 
    MatRadioModule, 
    MatButtonModule, 
    MatIconModule, 
    MatCardModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './info-adicional.html',
  styleUrls: ['./info-adicional.css']
})
export class InformacionAdicional implements OnInit {
  @Input() modoLectura: boolean = false;
  private fb = inject(FormBuilder);
  private ubigeoService = inject(UbigeoService);
  private postulanteInfoAdicionalService = inject(PostulanteInfoAdicionalService);
  private alertService = inject(AlertService);
  private authService = inject(AuthService);

  public infoForm!: FormGroup;
  public cargando = signal<boolean>(false);
  public listaDepartamentos = signal<any[]>([]);
  private idPostulante!: number;

  ngOnInit(): void {
    this.cargando.set(true);
    this.idPostulante = this.authService.obtenerIdPostulanteDesdeJwt();
    
    if (this.idPostulante > 0) {
      this.crearFormulario();
      this.escucharCambiosDisponibilidad();
      this.cargarDatosIniciales();
    } else {
      this.alertService.error('Error de Sesión', 'No se pudo identificar al postulante. Por favor reinicie sesión.');
    }
  }

  crearFormulario(): void {
    this.infoForm = this.fb.group({
      disponibilidadInterior: [false, [Validators.required]],
      departamentosIds: [[]] 
    });
  }

  cargarDatosIniciales(): void {
    forkJoin({
      departamentos: this.ubigeoService.getDepartamentos(),
      infoAdicional: this.postulanteInfoAdicionalService.getInfoAdicional(this.idPostulante)
    }).subscribe({
      next: (res) => {
        if (res.departamentos.success) {
          this.listaDepartamentos.set(res.departamentos.data);
        }

        if (res.infoAdicional.success && res.infoAdicional.data) {
          this.infoForm.patchValue({
            disponibilidadInterior: res.infoAdicional.data.disponibilidadInterior,
            departamentosIds: res.infoAdicional.data.departamentosIds || []
          });

          if (this.modoLectura) {
            this.infoForm.disable({ emitEvent: false });
          }
        }

        this.cargando.set(false);
      },
      error: (err) => {
        this.cargando.set(false);
        this.alertService.error('Error del Sistema', 'No se pudieron recuperar los catálogos de geolocalización.');
      }
    });
  }

  escucharCambiosDisponibilidad(): void {
    this.infoForm.get('disponibilidadInterior')?.valueChanges.subscribe((tieneDisp: boolean) => {
      const depControl = this.infoForm.get('departamentosIds');
      if (tieneDisp) {
        depControl?.setValidators([Validators.required]);
      } else {
        depControl?.clearValidators();
        depControl?.setValue([]);
      }
      depControl?.updateValueAndValidity();
    });
  }

  onSubmit(): void {
    if (this.infoForm.invalid) {
      this.infoForm.markAllAsTouched();
      return;
    }

    this.cargando.set(true);
    const payload = {
      idPostulante: this.idPostulante,
      ...this.infoForm.value
    };

    this.postulanteInfoAdicionalService.guardarInfoAdicional(payload).subscribe({
      next: (res) => {
        if (res.success) {
          this.alertService.exito('¡Guardado!', 'La información adicional se actualizó correctamente.');
        } else {
          this.alertService.advertencia('Atención', res.message);
        }
        this.cargando.set(false);
      },
      error: () => {
        this.cargando.set(false);
        this.alertService.error('Error', 'No se pudo guardar la información.');
      }
    });
  }
}