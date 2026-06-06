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
  selector: 'app-modal-ofimatica',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatDialogModule, MatFormFieldModule, MatSelectModule, MatButtonModule, MatIconModule],
  templateUrl: './modal-ofimatica.html',
  styleUrls: ['../../formacion/modal-formacion/modal-formacion.css']
})
export class ModalOfimatica implements OnInit {
  private fb = inject(FormBuilder);
  private dialogRef = inject(MatDialogRef<ModalOfimatica>);
  public data = inject(MAT_DIALOG_DATA);
  private catalogoService = inject(CatalogoService);

  public ofiForm!: FormGroup;
  public isEdicion = false;

  public listaHerramientas = signal<any[]>([]);
  public listaNiveles = signal<any[]>([]);

  ngOnInit(): void {
    this.isEdicion = !!this.data?.elemento;
    this.cargarCatalogos();
    this.inicializarFormulario();
  }

  cargarCatalogos(): void {
    this.catalogoService.getValoresByCodigo('OFIMATICA').subscribe(res => {
      if (res.success) this.listaHerramientas.set(res.data);
    });
    this.catalogoService.getValoresByCodigo('NIVEL_IDIOMA').subscribe(res => {
      if (res.success) this.listaNiveles.set(res.data);
    });
  }

  inicializarFormulario(): void {
    this.ofiForm = this.fb.group({
      idHerramientaCat: [this.data?.elemento?.idHerramientaCat || '', [Validators.required]],
      idNivelCat: [this.data?.elemento?.idNivelCat || '', [Validators.required]]
    });
  }

  guardar(): void {
    if (this.ofiForm.invalid) return;
    this.dialogRef.close({
      accion: this.isEdicion ? 'MODIFICAR' : 'REGISTRAR',
      idPostulanteOfimatica: this.data?.elemento?.idPostulanteOfimatica || 0,
      ...this.ofiForm.value
    });
  }

  cancelar(): void { this.dialogRef.close(null); }
}
