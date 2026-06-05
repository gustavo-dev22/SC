import { CommonModule } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { CatalogoService } from '../../../../services/catalogo.service';
import { debounceTime, distinctUntilChanged, switchMap, tap } from 'rxjs';

@Component({
  selector: 'app-modal-certificacion',
  imports: [CommonModule, ReactiveFormsModule, MatDialogModule, MatFormFieldModule, MatInputModule, MatSelectModule, MatButtonModule, MatIconModule, MatAutocompleteModule],
  templateUrl: './modal-certificacion.html',
  styleUrl: './modal-certificacion.css',
})
export class ModalCertificacion implements OnInit {
  private fb = inject(FormBuilder);
  private dialogRef = inject(MatDialogRef<ModalCertificacion>);
  public data = inject(MAT_DIALOG_DATA);
  private catalogoService = inject(CatalogoService);

  public certForm!: FormGroup;
  public isEdicion = false;
  public listaTiposCapacitacion = signal<any[]>([]);
  public centrosEstudiosFiltrados = signal<any[]>([]);
  public cargandoCentros = signal<boolean>(false);

  ngOnInit(): void {
    this.isEdicion = !!this.data?.elemento;
    this.inicializarFormulario();
    this.cargarCombos();
    this.configurarBuscadorInstituciones();
  }

  inicializarFormulario(): void {
    let fechaFormateada = '';
    if (this.data?.elemento?.fechaEmision) {
      fechaFormateada = this.data.elemento.fechaEmision.split('T')[0];
    }

    this.certForm = this.fb.group({
      idTipoEstudioCat: [this.data?.elemento?.idTipoEstudioCat || '', [Validators.required]],
      nombreEstudio: [this.data?.elemento?.nombreEstudio || '', [Validators.required, Validators.minLength(4)]],
      institucion: [this.data?.elemento?.institucion || '', [Validators.required]],
      horasAcademicas: [this.data?.elemento?.horasAcademicas || '', [Validators.required, Validators.min(1)]],
      fechaEmision: [fechaFormateada, [Validators.required]]
    });
  }

  cargarCombos(): void {
    this.catalogoService.getValoresByCodigo('TIPO_CAPACITACION').subscribe(res => {
      if (res.success) this.listaTiposCapacitacion.set(res.data);
    });
  }

  configurarBuscadorInstituciones(): void {
    this.certForm.get('institucion')?.valueChanges.pipe(
      debounceTime(350),
      distinctUntilChanged(),
      tap(() => this.cargandoCentros.set(true)),
      switchMap(value => this.catalogoService.getCentrosEstudiosUnificados(typeof value === 'string' ? value : ''))
    ).subscribe({
      next: (res) => {
        if (res.success) this.centrosEstudiosFiltrados.set(res.data);
        this.cargandoCentros.set(false);
      },
      error: () => this.cargandoCentros.set(false)
    });
  }

  guardar(): void {
    if (this.certForm.invalid) return;
    this.dialogRef.close({
      accion: this.isEdicion ? 'MODIFICAR' : 'REGISTRAR',
      idCertificacion: this.data?.elemento?.idCertificacion || 0,
      ...this.certForm.value
    });
  }

  cancelar(): void { this.dialogRef.close(null); }
}
