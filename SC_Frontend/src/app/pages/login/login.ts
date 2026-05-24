import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { Router } from '@angular/router';
import { Auth } from '../../services/auth.service';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule, MatCardModule, MatIconModule, FormsModule, MatFormFieldModule, MatInputModule, MatButtonModule],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {

  private fb = inject(FormBuilder);
  private router = inject(Router);
  private _authService = inject(Auth);

  public ocultarContrasena = signal<boolean>(true);
  public cargando = signal<boolean>(false);
  public errorLogin = signal<string | null>(null);
  public esExterno = signal<boolean>(false);

  setTipoUsuario(val: boolean): void {
    this.esExterno.set(val);
  }

  public loginForm: FormGroup = this.fb.group({
    usuario: ['', [Validators.required, Validators.minLength(4)]],
    contrasena: ['', [Validators.required, Validators.minLength(6)]]
  });

  // Alternar visibilidad de contraseña mutando el Signal
  togglePassword(): void {
    this.ocultarContrasena.update(prev => !prev);
  }

  onSubmit(): void {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    this.cargando.set(true);
    this.errorLogin.set(null);

    const { usuario, contrasena } = this.loginForm.value;

    this._authService.login(usuario, contrasena, this.esExterno()).subscribe({
      next: (res) => {
        if (res.success) {
          // Almacenamos el token corporativo y la estructura unificada de menús
          sessionStorage.setItem('token', res.data.token);
          sessionStorage.setItem('user_profile', JSON.stringify(res.data));
          
          // ALERTA PREMIUM DE ÉXITO
          Swal.fire({
            title: '¡Acceso Concedido!',
            text: res.message, // "Autenticación concedida con éxito."
            icon: 'success',
            timer: 2000,
            showConfirmButton: false,
            heightAuto: false, // Evita parpadeos con Angular Material
            background: '#ffffff',
            iconColor: '#2a5298'
          }).then(() => {
            this.cargando.set(false);
            // Redirección directa al Dashboard asimilando los cambios en el DOM
            this.router.navigate(['/dashboard']);
          });
        } else {
          this.cargando.set(false);
          this.errorLogin.set(res.message);
        }
      },
      error: (err) => {
        this.cargando.set(false);
        
        // Si el backend responde un BadRequest(400), el JSON viaja dentro de err.error
        const mensajeError = err.error?.message || 'Error de conexión con el servidor de seguridad.';
        this.errorLogin.set(mensajeError);

        // ALERTA PREMIUM DE ERROR GLOBAL
        Swal.fire({
          title: 'Error de Autenticación',
          text: mensajeError, // "Usuario, contraseña o rol de acceso incorrecto."
          icon: 'error',
          allowEscapeKey: false,
          allowOutsideClick: false,
          confirmButtonText: 'Entendido',
          confirmButtonColor: '#1e3c72',
          heightAuto: false
        });
      }
    });
  }
}
