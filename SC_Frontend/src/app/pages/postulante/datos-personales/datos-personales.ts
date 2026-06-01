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
      // Campos de solo lectura legal (Bloqueados de fábrica)
      numDocumento: [{ value: '', disabled: true }],
      nombres: [{ value: '', disabled: true }],
      apellidoPaterno: [{ value: '', disabled: true }],
      apellidoMaterno: [{ value: '', disabled: true }],
      correo: [{ value: '', disabled: true }],
      
      // Campos complementarios editables
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
          this.idPostulanteLogueado = 1; // Salvaguarda en caso de error
          this.cargarDatosPerfil();
        }
      }
    }
  }

  cargarCatálogos(): void {
    // 🚀 AUTOMATIZADO: Ya no importa si en BD es 1003, 50 o 1. El código 'CAT_SEXO' es universal.
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
          // Si la fecha viene con estampa de tiempo, la formateamos a YYYY-MM-DD para el input tipo date
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
    // getRawValue extrae absolutamente todos los campos, incluyendo los deshabilitados
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
          Swal.fire('¡Actualizado!', res.message, 'success');
          this.cargarDatosPerfil();
        }
      },
      error: () => this.cargando.set(false)
    });
  }
}
