import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatRadioModule } from '@angular/material/radio';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { UbigeoService } from '../../../services/ubigeo.service';
import { PostulanteInfoAdicionalService } from '../../../services/postulante-info-adicional.service';
import { AlertService } from '../../../shared/services/alert.service';

@Component({
  selector: 'app-informacion-adicional',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatFormFieldModule, MatSelectModule, MatRadioModule, MatButtonModule, MatIconModule, MatCardModule],
  templateUrl: './info-adicional.html',
  styleUrls: ['./info-adicional.css']
})
export class InformacionAdicional implements OnInit {
  private fb = inject(FormBuilder);
  private ubigeoService = inject(UbigeoService);
  private postulanteInfoAdicionalService = inject(PostulanteInfoAdicionalService);
  private alertService = inject(AlertService);

  public infoForm!: FormGroup;
  public cargando = signal<boolean>(false);
  public listaDepartamentos = signal<any[]>([]);
  private idPostulante!: number;

  ngOnInit(): void {
    const profile = JSON.parse(sessionStorage.getItem('user_profile') || '{}');
    const tokenParts = atob(profile.token).split('-');
    this.idPostulante = Number(tokenParts[1]);
    this.crearFormulario();
    this.cargarDepartamentosMaestro();
    this.escucharCambiosDisponibilidad();
  }

  crearFormulario(): void {
    this.infoForm = this.fb.group({
      disponibilidadInterior: [false, [Validators.required]],
      departamentosIds: [[]] // Array de strings vacío por defecto
    });
  }

  cargarDepartamentosMaestro(): void {
    this.ubigeoService.getDepartamentos().subscribe(res => {
      if (res.success) {
        this.listaDepartamentos.set(res.data);
        this.recuperarDatosGuardados(); // Solo leemos de la BD una vez tengamos el catálogo listo
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
        depControl?.setValue([]); // Limpiamos la selección si marca que No
      }
      depControl?.updateValueAndValidity();
    });
  }

  recuperarDatosGuardados(): void {
    this.cargando.set(true);
    this.postulanteInfoAdicionalService.getInfoAdicional(this.idPostulante).subscribe({
      next: (res) => {
        if (res.success && res.data) {
          this.infoForm.patchValue({
            disponibilidadInterior: res.data.disponibilidadInterior,
            departamentosIds: res.data.departamentosIds || []
          });
        }
        this.cargando.set(false);
      },
      error: () => this.cargando.set(false)
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
