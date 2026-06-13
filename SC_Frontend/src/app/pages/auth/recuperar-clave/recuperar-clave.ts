import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../services/auth.service';
import { AlertService } from '../../../shared/services/alert.service';

@Component({
  selector: 'app-recuperar-clave',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatCardModule, MatFormFieldModule, MatInputModule, MatButtonModule, MatIconModule, RouterModule],
  templateUrl: './recuperar-clave.html'
})
export class RecuperarClave implements OnInit {
  private fb = inject(FormBuilder);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private authService = inject(AuthService);
  private alertService = inject(AlertService);

  public resetForm!: FormGroup;
  public cargando = signal<boolean>(false);
  private token: string = '';

  ngOnInit(): void {
    // 🚀 LECTURA PROTEGIDA: Extraemos el token obligatorio de la URL
    this.token = this.route.snapshot.queryParamMap.get('token') || '';

    if (!this.token) {
      this.alertService.advertencia('Acceso Inválido', 'No se detectó un token de seguridad válido. Solicite un nuevo enlace.');
      this.router.navigate(['/auth/login']);
      return;
    }

    this.crearFormularioRestablecer();
  }

  crearFormularioRestablecer(): void {
    this.resetForm = this.fb.group({
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', [Validators.required]]
    }, { validators: this.passwordMatchValidator });
  }

  passwordMatchValidator(g: FormGroup) {
    return g.get('password')?.value === g.get('confirmPassword')?.value
      ? null : { mismatch: true };
  }

  onSubmit(): void {
    if (this.resetForm.invalid) return;

    this.cargando.set(true);
    const nuevaClave = this.resetForm.get('password')?.value;

    this.authService.confirmarRestablecimiento(this.token, nuevaClave).subscribe({
      next: (res) => {
        this.cargando.set(false);
        this.alertService.exito('¡Éxito!', 'Su contraseña fue actualizada correctamente. Ya puede iniciar sesión.');
        this.router.navigate(['/auth/login']);
      },
      error: (err) => {
        this.cargando.set(false);
        this.alertService.error('Error', err.error?.message || 'El enlace caducó o es inválido.');
      }
    });
  }
}