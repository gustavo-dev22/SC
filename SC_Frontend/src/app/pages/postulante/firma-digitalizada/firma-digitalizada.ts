import { Component, inject, Input, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { PostulanteFirmaService } from '../../../services/postulante-firma.service';
import { AlertService } from '../../../shared/services/alert.service';

@Component({
  selector: 'app-firma-digitalizada',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatButtonModule, MatIconModule, MatProgressSpinnerModule],
  templateUrl: './firma-digitalizada.html',
  styleUrls: ['./firma-digitalizada.css']
})
export class FirmaDigitalizada implements OnInit {
  @Input() modoLectura: boolean = false;
  private postulanteFirmaService = inject(PostulanteFirmaService);
  private alertService = inject(AlertService);

  public cargando = signal<boolean>(false);
  public urlFirmaPreview = signal<string | null>(null); 
  private idPostulante!: number;

  ngOnInit(): void {
    const profile = JSON.parse(sessionStorage.getItem('user_profile') || '{}');
    const tokenParts = atob(profile.token).split('-');
    this.idPostulante = Number(tokenParts[1]);
    this.cargarFirmaExistente();
  }

  cargarFirmaExistente(): void {
    this.cargando.set(true);
    this.postulanteFirmaService.getFirma(this.idPostulante).subscribe({
      next: (res) => {
        if (res.success && res.data?.firmaBase64) {
          this.urlFirmaPreview.set(res.data.firmaBase64);
        }
        this.cargando.set(false);
      },
      error: () => this.cargando.set(false)
    });
  }

  onFileSelected(event: any): void {
    const archivo: File = event.target.files[0];
    if (!archivo) return;

    // Validación defensiva corporativa de formatos de imagen
    const formatosPermitidos = ['image/png', 'image/jpeg', 'image/jpg'];
    if (!formatosPermitidos.includes(archivo.type)) {
      this.alertService.error('Formato Inválido', 'Solo se permiten imágenes en formato PNG, JPG o JPEG.');
      return;
    }

    // Validación de peso máximo (Máximo 2MB para no saturar la BD)
    if (archivo.size > 2 * 1024 * 1024) {
      this.alertService.advertencia('Archivo muy pesado', 'La imagen de la firma no debe superar los 2MB.');
      return;
    }

    this.cargando.set(true);
    this.postulanteFirmaService.subirFirma(this.idPostulante, archivo).subscribe({
      next: (res) => {
        if (res.success) {
          this.alertService.exito('¡Firma Cargada!', res.message);
          
          // Generamos un preview inmediato local usando FileReader para evitar retrasos de red
          const reader = new FileReader();
          reader.onload = () => this.urlFirmaPreview.set(reader.result as string);
          reader.readAsDataURL(archivo);
        } else {
          this.alertService.advertencia('Atención', res.message);
        }
        this.cargando.set(false);
      },
      error: () => {
        this.cargando.set(false);
        this.alertService.error('Error', 'Ocurrió un fallo en el servidor al intentar subir la firma.');
      }
    });
  }
}