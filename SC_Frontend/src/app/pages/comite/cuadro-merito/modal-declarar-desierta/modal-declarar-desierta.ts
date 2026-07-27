import { Component, inject, signal, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { CatalogoService } from '../../../../services/catalogo.service';

@Component({
  selector: 'app-modal-declarar-desierta',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatSelectModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule
  ],
  templateUrl: './modal-declarar-desierta.html',
})
export class ModalDeclararDesierta implements OnInit {
  private fb = inject(FormBuilder);
  private dialogRef = inject(MatDialogRef<ModalDeclararDesierta>);
  public data = inject(MAT_DIALOG_DATA);
  private catalogoService = inject(CatalogoService);

  public desiertaForm!: FormGroup;
  public listaMotivos = signal<any[]>([]);

  ngOnInit(): void {
    this.desiertaForm = this.fb.group({
      idMotivoDesiertaCat: [null, [Validators.required]],
      sustentoDesierta: ['', [Validators.required, Validators.maxLength(500)]]
    });

    // Cargamos los motivos de deserción desde el catálogo 'MOTIVO_DESIERTA'
    this.catalogoService.getValoresByCodigo('MOTIVO_DESIERTA').subscribe(res => {
      if (res.success) this.listaMotivos.set(res.data);
    });
  }

  guardar(): void {
    if (this.desiertaForm.invalid) return;
    this.dialogRef.close(this.desiertaForm.value);
  }

  cancelar(): void {
    this.dialogRef.close(null);
  }
}
