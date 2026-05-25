import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';

@Component({
  selector: 'app-modal-form',
  imports: [CommonModule, ReactiveFormsModule, MatDialogModule, MatFormFieldModule, MatInputModule, MatButtonModule, MatSlideToggleModule],
  templateUrl: './modal-form.html',
  styleUrl: './modal-form.css',
})
export class ModalForm {
  private fb = inject(FormBuilder);
  private dialogRef = inject(MatDialogRef<ModalForm>);
  public dataData = inject(MAT_DIALOG_DATA); // Recibe los datos del renglón si es edición

  public tituloForm = signal<string>(this.dataData.elemento ? 'Modificar Registro' : 'Nuevo Registro');
  
  public maestroForm: FormGroup = this.fb.group({
    idValor: [this.dataData.elemento?.idValor || 0],
    idTipo: [this.dataData.idTipo],
    codigoValor: [this.dataData.elemento?.codigoValor || '', [Validators.required]],
    descripcion: [this.dataData.elemento?.descripcion || '', [Validators.required]],
    orden: [this.dataData.elemento?.orden || 1, [Validators.required, Validators.min(1)]],
    activo: [this.dataData.elemento?.activo ?? true]
  });

  guardar(): void {
    if (this.maestroForm.invalid) return;
    
    const payload = {
      accion: this.dataData.elemento ? 'MODIFICAR' : 'REGISTRAR',
      ...this.maestroForm.value
    };
    
    this.dialogRef.close(payload); // Devuelve los datos listos al componente principal
  }

  cancelar(): void {
    this.dialogRef.close(null);
  }
}
