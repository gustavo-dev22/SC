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
  public listaNivelesDecan = signal<any[]>([]);
  public esDeportistaCalificado = signal<boolean>(false);

  private readonly CODIGO_DECAN = 'DECAN';

  ngOnInit(): void {
    this.isEdicion = !!this.data?.elemento;
    this.inicializarFormulario();
    this.cargarCatalogos();
  }

  inicializarFormulario(): void {
    const idTipoInicial = this.data?.elemento?.idTipoRequisitoCat ? Number(this.data.elemento.idTipoRequisitoCat) : null;
    const idNivelInicial = this.data?.elemento?.idCatalogoNivelDecanCat ? Number(this.data.elemento.idCatalogoNivelDecanCat) : null;

    this.reqForm = this.fb.group({
      idTipoRequisitoCat: [idTipoInicial, [Validators.required]],
      idCatalogoNivelDecanCat: [idNivelInicial],
      descripcionDocumento: [this.data?.elemento?.descripcionDocumento || '', [Validators.required, Validators.maxLength(150)]],
      numeroRegistro: [this.data?.elemento?.numeroRegistro || '', [Validators.required, Validators.maxLength(50)]],
      fechaEmision: [this.data?.elemento?.fechaEmision ? this.data.elemento.fechaEmision.split('T')[0] : ''],
      fechaVencimiento: [this.data?.elemento?.fechaVencimiento ? this.data.elemento.fechaVencimiento.split('T')[0] : '']
    });
  }

  cargarCatalogos(): void {
    // 1. Cargamos catálogo general 'OTROS'
    this.catalogoService.getValoresByCodigo('OTROS').subscribe(resOtros => {
      if (resOtros.success) {
        this.listaTiposRequisitos.set(resOtros.data);

        const idTipoActual = Number(this.reqForm.get('idTipoRequisitoCat')?.value);
        if (idTipoActual) {
          this.evaluarYAplicarReglasDecan(idTipoActual);
        }

        // 2. Cargamos catálogo secundario 'NIVEL_DECAN'
        this.catalogoService.getValoresByCodigo('NIVEL_DECAN').subscribe(resDecan => {
          if (resDecan.success) {
            this.listaNivelesDecan.set(resDecan.data);

            const idNivelGuardado = this.data?.elemento?.idCatalogoNivelDecanCat ? Number(this.data.elemento.idCatalogoNivelDecanCat) : null;
            if (idNivelGuardado) {
              setTimeout(() => {
                this.reqForm.patchValue({
                  idCatalogoNivelDecanCat: idNivelGuardado
                });
                this.reqForm.get('idCatalogoNivelDecanCat')?.updateValueAndValidity();
              }, 50);
            }
          }
        });
      }
    });
  }

  compararIds(o1: any, o2: any): boolean {
    if (o1 === null || o2 === null || o1 === undefined || o2 === undefined) return false;
    return Number(o1) === Number(o2);
  }

  onTipoRequisitoChange(idSeleccionado: any): void {
    if (!idSeleccionado) {
      this.esDeportistaCalificado.set(false);
      this.limpiarValidacionDecan();
      return;
    }

    const idValor = Number(idSeleccionado);
    this.evaluarYAplicarReglasDecan(idValor);
  }

  private evaluarYAplicarReglasDecan(idTipoRequisito: number): void {
    const itemEncontrado = this.listaTiposRequisitos().find(x => Number(x.idValor) === idTipoRequisito);
    
    // Evaluamos contra tu código oficial 'DECAN' (o 'DC' como salvaguarda)
    const esDecan = itemEncontrado ? (itemEncontrado.codigoValor === this.CODIGO_DECAN || itemEncontrado.codigoValor === 'DC') : false;
    this.esDeportistaCalificado.set(esDecan);

    const controlNivel = this.reqForm.get('idCatalogoNivelDecanCat');
    if (esDecan) {
      controlNivel?.setValidators([Validators.required]);
    } else {
      this.limpiarValidacionDecan();
    }
    controlNivel?.updateValueAndValidity();
    this.reqForm.updateValueAndValidity();
  }

  private limpiarValidacionDecan(): void {
    const controlNivel = this.reqForm.get('idCatalogoNivelDecanCat');
    controlNivel?.clearValidators();
    controlNivel?.setValue(null);
    controlNivel?.updateValueAndValidity();
  }

  guardar(): void {
    if (this.reqForm.invalid) return;
    
    const formValue = this.reqForm.value;

    // 🚀 LÓGICA DE LIMPIEZA DE DATOS: Garantizamos envío numérico o null puro
    const idNivelParsed = (this.esDeportistaCalificado() && formValue.idCatalogoNivelDecanCat) 
                          ? Number(formValue.idCatalogoNivelDecanCat) 
                          : null;

    const payload = {
      accion: this.isEdicion ? 'MODIFICAR' : 'REGISTRAR',
      idRequisitoEspecial: this.data?.elemento?.idRequisitoEspecial || 0,
      idTipoRequisitoCat: Number(formValue.idTipoRequisitoCat),
      idCatalogoNivelDecanCat: idNivelParsed, 
      descripcionDocumento: formValue.descripcionDocumento,
      numeroRegistro: formValue.numeroRegistro,
      fechaEmision: formValue.fechaEmision || null,
      fechaVencimiento: formValue.fechaVencimiento || null
    };

    this.dialogRef.close(payload);
  }

  cancelar(): void { this.dialogRef.close(null); }
}