import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';

@Component({
  selector: 'app-modal-tipo',
  imports: [CommonModule, ReactiveFormsModule, MatDialogModule, MatFormFieldModule, MatInputModule, MatButtonModule],
  templateUrl: './modal-tipo.html',
  styleUrl: './modal-tipo.css',
})
export class ModalTipo implements OnInit {
  private fb = inject(FormBuilder);
  private dialogRef = inject(MatDialogRef<ModalTipo>);
  public data = inject(MAT_DIALOG_DATA);

  public tipoForm: FormGroup = this.fb.group({
    idTipo: [0],
    codigo: ['', [Validators.required]],
    nombre: ['', [Validators.required]],
    activo: [true]
  });

  ngOnInit(): void {
    if (this.data?.elemento) {
      this.tipoForm.patchValue(this.data.elemento);
      this.tipoForm.get('codigo')?.disable(); 
    }
  }

  guardar(): void {
    if (this.tipoForm.invalid) return;
    const accion = this.data?.elemento ? 'MODIFICAR' : 'REGISTRAR';
    this.dialogRef.close({ accion, ...this.tipoForm.getRawValue() });
  }

  cancelar(): void { this.dialogRef.close(null); }
}
