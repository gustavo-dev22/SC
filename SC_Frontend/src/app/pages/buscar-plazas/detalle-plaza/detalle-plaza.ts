import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-detalle-plaza',
  standalone: true,
  imports: [CommonModule, MatDialogModule, MatButtonModule, MatIconModule],
  templateUrl: './detalle-plaza.html',
  styleUrl: './detalle-plaza.css',
})
export class DetallePlaza implements OnInit {
  private dialogRef = inject(MatDialogRef<DetallePlaza>);
  private dialogData = inject(MAT_DIALOG_DATA);

  public data: any;
  public esVencida: boolean = false;

  ngOnInit(): void {
    // Extraemos de forma segura el payload enviado por el componente padre
    this.data = this.dialogData.plaza;
    this.esVencida = this.dialogData.esVencida;
  }

  postularDesdeDetalle(): void {
    // Cerramos devolviendo true para que el listado principal dispare la confirmación
    this.dialogRef.close(true);
  }
}
