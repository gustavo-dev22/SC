import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { LoginRequest } from '../../core/models/auth.model';
import Swal from 'sweetalert2';
import { ParametroService } from '../../services/parametro.service';

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule, MatCardModule, MatIconModule, FormsModule, MatFormFieldModule, MatInputModule, MatButtonModule, RouterModule],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login implements OnInit{

  private fb = inject(FormBuilder);
  private router = inject(Router);
  private parametroService = inject(ParametroService);
  private _authService = inject(AuthService);

  public ocultarContrasena = signal<boolean>(true);
  public cargando = signal<boolean>(false);
  public errorLogin = signal<string | null>(null);
  public esExterno = signal<boolean>(false);

  public loginForm!: FormGroup;

  ngOnInit(): void {
    this.loginForm = this.fb.group({
      usuario: ['', [Validators.required, Validators.minLength(4)]],
      contrasena: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

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

    // 🚀 CONTROL DE ACCESO ORIENTADO A POLÍTICAS DE MANTENIMIENTO:
    if (this.esExterno()) {
      this.parametroService.verificarMantenimientoPortal().subscribe({
        next: (res) => {
          if (res.success && res.enMantenimiento) {
            this.cargando.set(false);
            
            // Alerta informativa fluida usando SweetAlert
            Swal.fire({
              title: 'Plataforma en Mantenimiento',
              text: 'Estimado ciudadano, el portal se encuentra temporalmente fuera de servicio por actualización técnica de bases de los concursos vigentes.',
              icon: 'info',
              confirmButtonText: 'Entendido',
              confirmButtonColor: '#1e3c72',
              heightAuto: false
            }).then(() => {
              // Redirección fulminante a la pantalla estática informativa
              this.router.navigate(['/mantenimiento']);
            });
          } else {
            // Si el flag es 0, ejecutamos la autenticación tradicional
            this.procederAutenticacion();
          }
        },
        error: () => {
          // Fail-safe: si el API de parámetros falla por red, dejamos intentar el login por resguardo
          this.procederAutenticacion();
        }
      });
    } else {
      // Si el usuario es de tipo INSTITUCIONAL, se salta la validación y entra directo
      this.procederAutenticacion();
    }
  }

  private procederAutenticacion(): void {
    const { usuario, contrasena } = this.loginForm.value;

    const request: LoginRequest = {
      username:   usuario,
      password:   contrasena,
      isExternal: this.esExterno()
    };

    this._authService.login(request).subscribe({
      next: (res) => {
        if (res.success) {
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
