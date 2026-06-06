import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { CatalogoService } from '../../../../services/catalogo.service';

@Component({
  selector: 'app-modal-idioma',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatDialogModule, MatFormFieldModule, MatSelectModule, MatButtonModule, MatIconModule],
  templateUrl: './modal-idioma.html',
  styleUrls: ['../../formacion/modal-formacion/modal-formacion.css']
})
export class ModalIdiomaComponent implements OnInit {
  private fb = inject(FormBuilder);
  private dialogRef = inject(MatDialogRef<ModalIdiomaComponent>);
  public data = inject(MAT_DIALOG_DATA);
  private catalogoService = inject(CatalogoService);

  public idmForm!: FormGroup;
  public isEdicion = false;
  
  public listaIdiomas = signal<any[]>([]);
  public listaNiveles = signal<any[]>([]);

  ngOnInit(): void {
    this.isEdicion = !!this.data?.elemento;
    this.cargarCatalogos();
    this.inicializarFormulario();
  }

  cargarCatalogos(): void {
    this.catalogoService.getValoresByCodigo('IDIOMAS').subscribe(res => {
      if (res.success) this.listaIdiomas.set(res.data);
    });
    this.catalogoService.getValoresByCodigo('NIVEL_IDIOMA').subscribe(res => {
      if (res.success) this.listaNiveles.set(res.data);
    });
  }

  inicializarFormulario(): void {
    this.idmForm = this.fb.group({
      idIdiomaCat: [this.data?.elemento?.idIdiomaCat || '', [Validators.required]],
      idNivelHablaCat: [this.data?.elemento?.idNivelHablaCat || '', [Validators.required]],
      idNivelLecturaCat: [this.data?.elemento?.idNivelLecturaCat || '', [Validators.required]],
      idNivelEscrituraCat: [this.data?.elemento?.idNivelEscrituraCat || '', [Validators.required]]
    });
  }

  guardar(): void {
    if (this.idmForm.invalid) return;
    this.dialogRef.close({
      accion: this.isEdicion ? 'MODIFICAR' : 'REGISTRAR',
      idPostulanteIdioma: this.data?.elemento?.idPostulanteIdioma || 0,
      ...this.idmForm.value
    });
  }

  cancelar(): void { this.dialogRef.close(null); }
}