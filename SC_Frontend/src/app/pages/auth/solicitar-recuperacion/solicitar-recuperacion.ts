import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../services/auth.service';
import { AlertService } from '../../../shared/services/alert.service';

@Component({
  selector: 'app-solicitar-recuperacion',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatCardModule, MatFormFieldModule, MatInputModule, MatButtonModule, MatIconModule, RouterModule],
  templateUrl: './solicitar-recuperacion.html'
})
export class SolicitarRecuperacion {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private alertService = inject(AlertService);

  public recuperarForm: FormGroup;
  public cargando = signal<boolean>(false);
  public linkDesarrolloSimulado = signal<string>( ' ');

  constructor() {
    this.recuperarForm = this.fb.group({
      numDocumento: ['', [Validators.required, Validators.minLength(8), Validators.maxLength(15)]]
    });
  }

  enviarSolicitud(): void {
    if (this.recuperarForm.invalid) return;

    this.cargando.set(true);
    const dni = this.recuperarForm.get('numDocumento')?.value;

    this.authService.solicitarEnlaceRecuperacion(dni).subscribe({
      next: (res) => {
        this.cargando.set(false);
        this.alertService.exito('Solicitud Procesada', res.message);
        
        if (res.linkDesarrollo) {
          // 🚀 CORRECCIÓN: Forzamos dinámicamente que el link simulado apunte 
          // a la ruta exacta de tu componente destino: /auth/restablecer-password
          const linkCorregido = res.linkDesarrollo.replace('restablecer-password', 'restablecer-password');
          this.linkDesarrolloSimulado.set(linkCorregido);
        }
      },
      error: (err) => {
        this.cargando.set(false);
        this.alertService.error('Atención', err.error?.message || 'No se pudo procesar la solicitud.');
      }
    });
  }
}