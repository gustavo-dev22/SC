import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common'; // 🚀 Indispensable para el [ngClass]
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-modal-alerta',
  standalone: true,
  imports: [CommonModule, MatDialogModule, MatButtonModule, MatIconModule],
  templateUrl: './modal-alerta.html',
  styleUrls: ['./modal-alerta.css'] // 🚀 Vinculado en limpio
})
export class ModalAlertaComponent {
  private dialogRef = inject(MatDialogRef<ModalAlertaComponent>);
  public data = inject(MAT_DIALOG_DATA);

  cerrar(): void {
    this.dialogRef.close();
  }
}