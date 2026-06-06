import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { CatalogoService } from '../../../../services/catalogo.service'; // Tu servicio de listas/parámetros

@Component({
  selector: 'app-modal-colegiatura',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatDialogModule, MatFormFieldModule, MatInputModule, MatSelectModule, MatButtonModule, MatIconModule],
  templateUrl: './modal-colegiatura.html',
  styleUrls: ['../../formacion/modal-formacion/modal-formacion.css'] // Reutiliza la maquetación limpia de los modales
})
export class ModalColegiatura implements OnInit {
  private fb = inject(FormBuilder);
  private dialogRef = inject(MatDialogRef<ModalColegiatura>);
  public data = inject(MAT_DIALOG_DATA);
  private catalogoService = inject(CatalogoService);

  public colForm!: FormGroup;
  public isEdicion = false;
  public listaColegios = signal<any[]>([]); // Semilla de catálogos (CIP, CAL, etc.)

  ngOnInit(): void {
    this.isEdicion = !!this.data?.elemento;
    this.cargarCatalogoColegios();
    this.inicializarFormulario();
  }

  cargarCatalogoColegios(): void {
    // Llamas a tu endpoint de sc_catalogo_valor pasando el código correspondiente a Consejos Profesionales
    this.catalogoService.getValoresByCodigo('COLEGIOS_PROFESIONALES').subscribe(res => {
      if (res.success) this.listaColegios.set(res.data);
    });
  }

  inicializarFormulario(): void {
    this.colForm = this.fb.group({
      idColegioCat: [this.data?.elemento?.idColegioCat || '', [Validators.required]],
      numeroColegiacion: [this.data?.elemento?.numeroColegiacion || '', [Validators.required, Validators.maxLength(30)]],
      fechaColegiacion: [this.data?.elemento?.fechaColegiacion ? this.data.elemento.fechaColegiacion.split('T')[0] : '', [Validators.required]]
    });
  }

  guardar(): void {
    if (this.colForm.invalid) return;
    this.dialogRef.close({
      accion: this.isEdicion ? 'MODIFICAR' : 'REGISTRAR',
      idColegiatura: this.data?.elemento?.idColegiatura || 0,
      ...this.colForm.value,
      certificadoHabilitacionRuta: '' // Dejado en blanco para cuando implementemos el file-uploader general
    });
  }

  cancelar(): void { this.dialogRef.close(null); }
}
