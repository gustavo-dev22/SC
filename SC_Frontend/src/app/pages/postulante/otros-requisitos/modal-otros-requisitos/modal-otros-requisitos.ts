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

@Component({
  selector: 'app-modal-otros-requisitos',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatDialogModule, MatFormFieldModule, MatInputModule, MatSelectModule, MatButtonModule, MatIconModule],
  templateUrl: './modal-otros-requisitos.html',
  styleUrls: ['../../formacion/modal-formacion/modal-formacion.css']
})
export class ModalOtrosRequisitos implements OnInit {
  private fb = inject(FormBuilder);
  private dialogRef = inject(MatDialogRef<ModalOtrosRequisitos>);
  public data = inject(MAT_DIALOG_DATA);
  private catalogoService = inject(CatalogoService);

  public reqForm!: FormGroup;
  public isEdicion = false;
  public listaTiposRequisitos = signal<any[]>([]);

  ngOnInit(): void {
    this.isEdicion = !!this.data?.elemento;
    this.cargarCatalogos();
    this.inicializarFormulario();
  }

  cargarCatalogos(): void {
    this.catalogoService.getValoresByCodigo('OTROS').subscribe(res => {
      if (res.success) this.listaTiposRequisitos.set(res.data);
    });
  }

  inicializarFormulario(): void {
    this.reqForm = this.fb.group({
      idTipoRequisitoCat: [this.data?.elemento?.idTipoRequisitoCat || '', [Validators.required]],
      descripcionDocumento: [this.data?.elemento?.descripcionDocumento || '', [Validators.required, Validators.maxLength(150)]],
      numeroRegistro: [this.data?.elemento?.numeroRegistro || '', [Validators.required, Validators.maxLength(50)]],
      fechaEmision: [this.data?.elemento?.fechaEmision ? this.data.elemento.fechaEmision.split('T')[0] : ''],
      fechaVencimiento: [this.data?.elemento?.fechaVencimiento ? this.data.elemento.fechaVencimiento.split('T')[0] : '']
    });
  }

  guardar(): void {
    if (this.reqForm.invalid) return;
    
    const formValue = this.reqForm.value;
    this.dialogRef.close({
      accion: this.isEdicion ? 'MODIFICAR' : 'REGISTRAR',
      idRequisitoEspecial: this.data?.elemento?.idRequisitoEspecial || 0,
      ...formValue,
      fechaEmision: formValue.fechaEmision || null,
      fechaVencimiento: formValue.fechaVencimiento || null
    });
  }

  cancelar(): void { this.dialogRef.close(null); }
}