import { Component, Inject, OnInit, ViewChild, signal } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-historial-tickets-modal',
  standalone: true, // Lo hacemos autocontenido para fácil importación
  imports: [
    CommonModule, 
    MatDialogModule, 
    MatTableModule, 
    MatPaginatorModule, 
    MatButtonModule, 
    MatIconModule
  ],
  templateUrl: './historial-tickets-modal.html',
  styleUrl: './historial-tickets-modal.css',
})
export class HistorialTicketsModal implements OnInit {
  public columnas: string[] = ['ticket', 'asunto', 'fecha', 'estado'];
  public dataSource = new MatTableDataSource<any>([]);

  @ViewChild(MatPaginator, { static: true }) paginator!: MatPaginator;

  constructor(
    public dialogRef: MatDialogRef<HistorialTicketsModal>,
    @Inject(MAT_DIALOG_DATA) public data: { tickets: any[] }
  ) {}

  ngOnInit(): void {
    // Inyectamos la data completa en la tabla
    this.dataSource.data = this.data.tickets;
    this.dataSource.paginator = this.paginator;
  }

  cerrarModal(): void {
    this.dialogRef.close();
  }
}