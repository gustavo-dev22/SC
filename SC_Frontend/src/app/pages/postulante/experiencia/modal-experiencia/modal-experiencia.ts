import { CommonModule } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { CatalogoService } from '../../../../services/catalogo.service';
import { debounceTime, distinctUntilChanged, switchMap, tap } from 'rxjs';
import Swal from 'sweetalert2';
import { AlertService } from '../../../../shared/services/alert.service';

@Component({
  selector: 'app-modal-experiencia',
  imports: [CommonModule, ReactiveFormsModule, MatDialogModule, MatFormFieldModule, MatInputModule, MatButtonModule, MatIconModule, MatCheckboxModule, MatAutocompleteModule],
  templateUrl: './modal-experiencia.html',
  styleUrl: './modal-experiencia.css',
})
export class ModalExperiencia implements OnInit {
  private fb = inject(FormBuilder);
  private dialogRef = inject(MatDialogRef<ModalExperiencia>);
  public data = inject(MAT_DIALOG_DATA);
  private catalogoService = inject(CatalogoService);
  private alertService = inject(AlertService);

  public expForm!: FormGroup;
  public isEdicion = false;

  public centrosEstudiosFiltrados = signal<any[]>([]);
  public cargandoCentros = signal<boolean>(false);

  ngOnInit(): void {
    this.isEdicion = !!this.data?.elemento;
    this.inicializarFormulario();
    this.configurarBuscadorEntidades();
  }

  inicializarFormulario(): void {
    this.expForm = this.fb.group({
      empresaInstitucion: [this.data?.elemento?.empresaInstitucion || '', [Validators.required]],
      cargoPuesto: [this.data?.elemento?.cargoPuesto || '', [Validators.required]],
      fechaInicio: [this.data?.elemento?.fechaInicio ? this.data.elemento.fechaInicio.split('T')[0] : '', [Validators.required]],
      fechaFin: [this.data?.elemento?.fechaFin ? this.data.elemento.fechaFin.split('T')[0] : null],
      esSectorPublico: [this.data?.elemento?.esSectorPublico || false],
      esExperienciaEspecifica: [this.data?.elemento?.esExperienciaEspecifica || false],
      funcionesPrincipales: [this.data?.elemento?.funcionesPrincipales || '', [Validators.required, Validators.maxLength(1000)]]
    });
  }

  configurarBuscadorEntidades(): void {
    this.expForm.get('empresaInstitucion')?.valueChanges.pipe(
      debounceTime(350),
      distinctUntilChanged(),
      tap(() => this.cargandoCentros.set(true)),
      switchMap(value => {
        const textoBuscar = typeof value === 'string' ? value : '';
        return this.catalogoService.getCentrosEstudiosUnificados(textoBuscar);
      })
    ).subscribe({
      next: (res) => {
        if (res.success && Array.isArray(res.data)) {
          this.centrosEstudiosFiltrados.set(res.data);
        }
        this.cargandoCentros.set(false);
      },
      error: () => this.cargandoCentros.set(false)
    });
  }

  guardar(): void {
    if (this.expForm.invalid) return;
    
    const formValue = this.expForm.value;
    const idExperienciaActual = this.data?.elemento?.idExperiencia ?? this.data?.elemento?.IdExperiencia ?? 0;

    const nuevaFechaInicio = new Date(formValue.fechaInicio + 'T00:00:00');
    const nuevaFechaFin = formValue.fechaFin ? new Date(formValue.fechaFin + 'T00:00:00') : new Date();

    if (formValue.fechaFin && nuevaFechaInicio > nuevaFechaFin) {
      this.alertService.error('Error de Fechas', 'La fecha de inicio no puede ser posterior a la fecha de término.');
      return;
    }

    const listaAValidar = this.data?.listaActual || [];
    
    const hayCruce = listaAValidar.some((exp: any) => {
      const idItemLista = exp.idExperiencia ?? exp.IdExperiencia ?? 0;
      
      if (idItemLista > 0 && idItemLista === idExperienciaActual) {
        return false; 
      }

      const expInicio = new Date(exp.fechaInicio);
      const expFin = exp.fechaFin ? new Date(exp.fechaFin) : new Date();

      return nuevaFechaInicio <= expFin && nuevaFechaFin >= expInicio;
    });

    if (hayCruce) {
      this.alertService.advertencia('Periodo Duplicado', 'No es posible registrar esta experiencia laboral. Las fechas ingresadas se superponen con otro periodo.');
      return;
    }

    this.dialogRef.close({
      accion: this.isEdicion ? 'MODIFICAR' : 'REGISTRAR',
      idExperiencia: idExperienciaActual, 
      IdExperiencia: idExperienciaActual, 
      ...formValue,
      fechaFin: formValue.fechaFin || null
    });
  }

  cancelar(): void { this.dialogRef.close(null); }
}
