import { Component, inject, signal } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule, MatCardModule, MatIconModule, FormsModule, MatFormFieldModule, MatInputModule, MatButtonModule, RouterModule],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {

  private fb = inject(FormBuilder);
  private router = inject(Router);
  private _authService = inject(AuthService);

  public ocultarContrasena = signal<boolean>(true);
  public cargando = signal<boolean>(false);
  public errorLogin = signal<string | null>(null);
  public esExterno = signal<boolean>(false);

  setTipoUsuario(esCiudadano: boolean): void {
    this.esExterno.set(esCiudadano);
    this.errorLogin.set(null);
    
    this.loginForm.reset({ usuario: '', contrasena: '' });
    
    const usuarioControl = this.loginForm.get('usuario');
    if (esCiudadano) {
      usuarioControl?.setValidators([Validators.required, Validators.minLength(8), Validators.maxLength(12)]);
    } else {
      usuarioControl?.setValidators([Validators.required, Validators.minLength(4)]);
    }
    usuarioControl?.updateValueAndValidity();
  }

  public loginForm: FormGroup = this.fb.group({
    usuario: ['', [Validators.required, Validators.minLength(4)]],
    contrasena: ['', [Validators.required, Validators.minLength(6)]]
  });

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
          sessionStorage.setItem('token', res.data.token);
          sessionStorage.setItem('user_profile', JSON.stringify(res.data));
          
          // ALERTA PREMIUM DE ÉXITO
          Swal.fire({
            title: '¡Acceso Concedido!',
            text: res.message, 
            icon: 'success',
            timer: 2000,
            showConfirmButton: false,
            heightAuto: false, 
            background: '#ffffff',
            iconColor: '#2a5298'
          }).then(() => {
            this.cargando.set(false);
            this.router.navigate(['/dashboard']);
          });
        } else {
          this.cargando.set(false);
          this.errorLogin.set(res.message);
        }
      },
      error: (err) => {
        this.cargando.set(false);
        
        const mensajeError = err.error?.message || 'Error de conexión con el servidor de seguridad.';
        this.errorLogin.set(mensajeError);

        Swal.fire({
          title: 'Error de Autenticación',
          text: mensajeError, 
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
