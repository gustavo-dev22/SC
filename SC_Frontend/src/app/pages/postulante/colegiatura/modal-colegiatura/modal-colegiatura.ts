import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { CatalogoService } from '../../../../services/catalogo.service';
import { MatRadioModule } from '@angular/material/radio';

@Component({
  selector: 'app-modal-colegiatura',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatDialogModule, MatFormFieldModule, MatInputModule, MatSelectModule, MatButtonModule, MatIconModule, MatRadioModule],
  templateUrl: './modal-colegiatura.html',
  styleUrls: ['../../formacion/modal-formacion/modal-formacion.css']
})
export class ModalColegiatura implements OnInit {
  private fb = inject(FormBuilder);
  private dialogRef = inject(MatDialogRef<ModalColegiatura>);
  public data = inject(MAT_DIALOG_DATA);
  private catalogoService = inject(CatalogoService);

  public colForm!: FormGroup;
  public isEdicion = false;
  public listaColegios = signal<any[]>([]);

  ngOnInit(): void {
    this.isEdicion = !!this.data?.elemento;
    this.cargarCatalogoColegios();
    this.inicializarFormulario();
    this.escucharCambiosHabilitacion();
  }

  cargarCatalogoColegios(): void {
    this.catalogoService.getValoresByCodigo('COLEGIOS_PROFESIONALES').subscribe(res => {
      if (res.success) this.listaColegios.set(res.data);
    });
  }

  inicializarFormulario(): void {
    const elemento = this.data?.elemento;
    const esHabilitado = elemento ? elemento.habilitado : true;

    this.colForm = this.fb.group({
      idColegioCat: [this.data?.elemento?.idColegioCat || '', [Validators.required]],
      numeroColegiacion: [this.data?.elemento?.numeroColegiacion || '', [Validators.required, Validators.maxLength(30)]],
      fechaColegiacion: [this.data?.elemento?.fechaColegiacion ? this.data.elemento.fechaColegiacion.split('T')[0] : '', [Validators.required]],
      habilitado: [esHabilitado, [Validators.required]],
      motivoNoHabilitado: [elemento?.motivoNoHabilitado || '', elemento?.habilitado === false ? [Validators.required, Validators.maxLength(250)] : [Validators.maxLength(250)]]
    });
  }

  escucharCambiosHabilitacion(): void {
    this.colForm.get('habilitado')?.valueChanges.subscribe((estaHabilitado: boolean) => {
      const motivoControl = this.colForm.get('motivoNoHabilitado');
      
      if (estaHabilitado) {
        motivoControl?.clearValidators();
        motivoControl?.setValue('');
        motivoControl?.setValidators([Validators.maxLength(250)]);
      } else {
        motivoControl?.setValidators([Validators.required, Validators.maxLength(250)]);
      }
      
      motivoControl?.updateValueAndValidity();
    });
  }

  guardar(): void {
    if (this.colForm.invalid) return;
    this.dialogRef.close({
      accion: this.isEdicion ? 'MODIFICAR' : 'REGISTRAR',
      idColegiatura: this.data?.elemento?.idColegiatura || 0,
      ...this.colForm.value,
      certificadoHabilitacionRuta: ''
    });
  }

  cancelar(): void { this.dialogRef.close(null); }
}
