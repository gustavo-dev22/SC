import { CommonModule } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import Swal from 'sweetalert2';
import { PostulantePerfilService } from '../../../services/postulante-perfil.service';
import { CatalogoService } from '../../../services/catalogo.service';
import { AlertService } from '../../../shared/services/alert.service';

@Component({
  selector: 'app-datos-personales',
  imports: [
    CommonModule, ReactiveFormsModule, MatCardModule, MatFormFieldModule,
    MatInputModule, MatSelectModule, MatButtonModule, MatIconModule, MatProgressSpinnerModule
  ],
  templateUrl: './datos-personales.html',
  styleUrl: './datos-personales.css',
})
export class DatosPersonales implements OnInit {
  private fb = inject(FormBuilder);
  private perfilService = inject(PostulantePerfilService);
  private catalogoService = inject(CatalogoService);
  private alertService = inject(AlertService);

  public perfilForm!: FormGroup;
  public cargando = signal<boolean>(false);
  public listaSexo = signal<any[]>([]); 
  private idPostulanteLogueado!: number;

  ngOnInit(): void {
    this.inicializarFormulario();
    this.recuperarIdSesion();
    this.cargarCatálogos();
  }

  inicializarFormulario(): void {
    this.perfilForm = this.fb.group({
      numDocumento: [{ value: '', disabled: true }],
      nombres: [{ value: '', disabled: true }],
      apellidoPaterno: [{ value: '', disabled: true }],
      apellidoMaterno: [{ value: '', disabled: true }],
      correo: [{ value: '', disabled: true }],
      telefono: ['', [Validators.required, Validators.pattern('^[0-9]{9}$')]],
      fechaNacimiento: ['', [Validators.required]],
      idSexoCat: [0, [Validators.required, Validators.min(1)]],
      direccion: ['', [Validators.required, Validators.minLength(10)]]
    });
  }

  recuperarIdSesion(): void {
    const rawProfile = sessionStorage.getItem('user_profile');
    if (rawProfile) {
      const profile = JSON.parse(rawProfile);
      
      if (profile.token) {
        try {
          const decodedToken = atob(profile.token);
          const tokenParts = decodedToken.split('-');
          this.idPostulanteLogueado = Number(tokenParts[1]);
          this.cargarDatosPerfil();
        } catch (error) {
          console.error('Error al descifrar el token del postulante:', error);
          this.idPostulanteLogueado = 1;
          this.cargarDatosPerfil();
        }
      }
    }
  }

  cargarCatálogos(): void {
    this.catalogoService.getValoresByCodigo('SEXO').subscribe({
      next: (res) => { 
        if (res.success) {
          this.listaSexo.set(res.data); 
        } 
      }
    });
  }

  cargarDatosPerfil(): void {
    this.cargando.set(true);
    this.perfilService.getPerfil(this.idPostulanteLogueado).subscribe({
      next: (res) => {
        if (res.success && res.data) {
          if (res.data.fechaNacimiento) {
            res.data.fechaNacimiento = res.data.fechaNacimiento.split('T')[0];
          }
          this.perfilForm.patchValue(res.data);
        }
        this.cargando.set(false);
      },
      error: () => this.cargando.set(false)
    });
  }

  onSubmit(): void {
    if (this.perfilForm.invalid) {
      this.perfilForm.markAllAsTouched();
      return;
    }

    this.cargando.set(true);

    const formValues = this.perfilForm.getRawValue();
    
    const payload = {
      idPostulante: this.idPostulanteLogueado,
      telefono: formValues.telefono,
      fechaNacimiento: formValues.fechaNacimiento,
      idSexoCat: formValues.idSexoCat,
      direccion: formValues.direccion
    };

    this.perfilService.updatePerfil(payload).subscribe({
      next: (res) => {
        if (res.success) {
          this.alertService.exito('¡Actualizado!', res.message || 'Los datos de su perfil se actualizaron correctamente.');
          this.cargarDatosPerfil();
        } else {
          this.alertService.advertencia('Atención', res.message || 'No se pudo completar la actualización.');
          this.cargando.set(false);
        }
      },
      error: (err) => {
        this.cargando.set(false);
        this.alertService.error('Error del Sistema', 'Ocurrió un problema inesperado al comunicar con el servidor. Inténtelo nuevamente.');
      }
    });
  }
}
