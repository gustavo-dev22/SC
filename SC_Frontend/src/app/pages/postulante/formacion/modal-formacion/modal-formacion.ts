import { CommonModule } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { CatalogoService } from '../../../../services/catalogo.service';
import { MatIconModule } from '@angular/material/icon';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { debounceTime, distinctUntilChanged, switchMap, tap } from 'rxjs';

@Component({
  selector: 'app-modal-formacion',
  imports: [CommonModule, ReactiveFormsModule, MatDialogModule, MatFormFieldModule, MatInputModule, MatSelectModule, MatButtonModule, MatIconModule, MatAutocompleteModule],
  templateUrl: './modal-formacion.html',
  styleUrl: './modal-formacion.css',
})
export class ModalFormacion implements OnInit {
  private fb = inject(FormBuilder);
  private dialogRef = inject(MatDialogRef<ModalFormacion>);
  public data = inject(MAT_DIALOG_DATA);
  private catalogoService = inject(CatalogoService);

  public formForm!: FormGroup;
  public isEdicion = false;
  
  public listaNiveles = signal<any[]>([]);
  public listaEstados = signal<any[]>([]);
  
  public meses = [
    { id: 1, name: 'Enero' }, { id: 2, name: 'Febrero' }, { id: 3, name: 'Marzo' }, { id: 4, name: 'Abril' },
    { id: 5, name: 'Mayo' }, { id: 6, name: 'Junio' }, { id: 7, name: 'Julio' }, { id: 8, name: 'Agosto' },
    { id: 9, name: 'Setiembre' }, { id: 10, name: 'Octubre' }, { id: 11, name: 'Noviembre' }, { id: 12, name: 'Diciembre' }
  ];

  public listaUniversidades = signal<string[]>([]);
  public universidadesFiltradas = signal<string[]>([]);

  public centrosEstudiosFiltrados = signal<any[]>([]);
  public cargandoCentros = signal<boolean>(false);

  ngOnInit(): void {
    this.isEdicion = !!this.data?.elemento;
    this.inicializarFormulario();
    this.cargarCombos();
    this.configurarBuscadorPredictivo();
  }

  inicializarFormulario(): void {
    this.formForm = this.fb.group({
      idNivelCat: [this.data?.elemento?.idNivelCat || '', [Validators.required]],
      idEstadoCat: [this.data?.elemento?.idEstadoCat || '', [Validators.required]],
      institucion: [this.data?.elemento?.institucion || '', [Validators.required]],
      carrera: [this.data?.elemento?.carrera || '', [Validators.required]],
      mesInicio: [this.data?.elemento?.mesInicio || '', [Validators.required]],
      anioInicio: [this.data?.elemento?.anioInicio || '', [Validators.required, Validators.min(1950)]],
      mesFin: [this.data?.elemento?.mesFin || null],
      anioFin: [this.data?.elemento?.anioFin || null]
    });
  }

  cargarCombos(): void {
    this.catalogoService.getValoresByCodigo('NIVEL_ESTUDIO').subscribe(res => { if(res.success) this.listaNiveles.set(res.data); });
    this.catalogoService.getValoresByCodigo('ESTADO_ESTUDIO').subscribe(res => { if(res.success) this.listaEstados.set(res.data); });
  }

  configurarBuscadorPredictivo(): void {
    this.formForm.get('institucion')?.valueChanges.pipe(
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
    if (this.formForm.invalid) return;
    this.dialogRef.close({
      accion: this.isEdicion ? 'MODIFICAR' : 'REGISTRAR',
      idFormacion: this.data?.elemento?.idFormacion || 0,
      ...this.formForm.value
    });
  }

  cancelar(): void { this.dialogRef.close(null); }
}
