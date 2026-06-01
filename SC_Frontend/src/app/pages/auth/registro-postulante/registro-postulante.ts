import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { Router, RouterLink } from '@angular/router';
import Swal from 'sweetalert2';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-registro-postulante',
  imports: [CommonModule, ReactiveFormsModule, RouterLink, MatCardModule, 
    MatFormFieldModule, MatInputModule, MatButtonModule, MatIconModule, 
    MatProgressSpinnerModule],
  templateUrl: './registro-postulante.html',
  styleUrl: './registro-postulante.css',
})
export class RegistroPostulante {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private router = inject(Router);

  public cargando = signal<boolean>(false);
  public ocultarClave = signal<boolean>(true);

  public registroForm: FormGroup = this.fb.group({
    numDocumento: ['', [Validators.required, Validators.pattern('^[0-9]{8,11}$')]], // DNI (8 dig) o RUC/CE
    nombres: ['', [Validators.required]],
    apellidoPaterno: ['', [Validators.required]],
    apellidoMaterno: ['', [Validators.required]],
    correo: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]]
  });

  onSubmit(): void {
    if (this.registroForm.invalid) {
      this.registroForm.markAllAsTouched();
      return;
    }

    this.cargando.set(true);
    this.authService.registrarPostulante(this.registroForm.value).subscribe({
      next: (res) => {
        if (res.success) {
          Swal.fire({
            title: '¡Registro Exitoso!',
            text: res.message,
            icon: 'success',
            confirmButtonText: 'Ir al Login',
            confirmButtonColor: '#1e3c72',
            heightAuto: false
          }).then(() => {
            this.router.navigate(['/login']); // Redirige al login listo para acceder
          });
        }
        this.cargando.set(false);
      },
      error: (err) => {
        this.cargando.set(false);
        const errorMsg = err.error?.message || 'Ocurrió un percance al procesar el registro.';
        Swal.fire('Error de Registro', errorMsg, 'error');
      }
    });
  }
}
