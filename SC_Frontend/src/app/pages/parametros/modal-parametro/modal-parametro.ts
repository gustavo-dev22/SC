import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';

@Component({
  selector: 'app-modal-parametro',
  imports: [CommonModule, ReactiveFormsModule, MatDialogModule, MatFormFieldModule, MatInputModule, MatButtonModule],
  templateUrl: './modal-parametro.html',
  styleUrl: './modal-parametro.css',
})
export class ModalParametro implements OnInit {
  private fb = inject(FormBuilder);
  private dialogRef = inject(MatDialogRef<ModalParametro>);
  public data = inject(MAT_DIALOG_DATA);

  public isEdicion = false;
  public paramForm!: FormGroup;

  ngOnInit(): void {
    this.isEdicion = !!this.data?.elemento;
    
    this.paramForm = this.fb.group({
      codigo: [{ value: this.data?.elemento?.codigo || '', disabled: this.isEdicion }, [Validators.required]],
      nombre: [this.data?.elemento?.nombre || '', [Validators.required]],
      valor: [this.data?.elemento?.valor || '', [Validators.required]],
      descripcion: [this.data?.elemento?.descripcion || '', [Validators.required]],
      categoria: [this.data?.elemento?.categoria || 'SISTEMA', [Validators.required]]
    });
  }

  guardar(): void {
    if (this.paramForm.invalid) return;
    this.dialogRef.close({
      accion: this.isEdicion ? 'MODIFICAR' : 'REGISTRAR',
      ...this.paramForm.getRawValue()
    });
  }

  cancelar(): void { this.dialogRef.close(null); }
}
