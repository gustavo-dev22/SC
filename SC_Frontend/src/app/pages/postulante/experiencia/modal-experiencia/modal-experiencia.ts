import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { debounceTime, distinctUntilChanged, switchMap, tap } from 'rxjs/operators';
import { CatalogoService } from '../../../../services/catalogo.service';
import { AlertService } from '../../../../shared/services/alert.service';
import { forkJoin } from 'rxjs';

@Component({
  selector: 'app-modal-experiencia',
  imports: [CommonModule, ReactiveFormsModule, MatDialogModule, MatFormFieldModule, MatInputModule, MatButtonModule, MatIconModule, MatCheckboxModule, MatAutocompleteModule, MatSelectModule],
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

  public listaSectores = signal<any[]>([]);
  public listaRegimenes = signal<any[]>([]);
  public listaMotivos = signal<any[]>([]);

  ngOnInit(): void {
    this.isEdicion = !!this.data?.elemento;
    
    // 🚀 PASO 1: Inicialización INSTANTÁNEA del formulario vacío para blindar el HTML
    this.crearFormularioVacio();
    
    // 🚀 PASO 2: Configuración del debounce del buscador
    this.configurarBuscadorEntidades();

    // 🚀 PASO 3: Carga paralela de catálogos y parchado controlado
    this.cargandoCentros.set(true);
    forkJoin({
      sectores: this.catalogoService.getValoresByCodigo('SEC'),
      motivos: this.catalogoService.getValoresByCodigo('MOTIVO_CAMBIO')
    }).subscribe({
      next: (res) => {
        if (res.sectores.success) this.listaSectores.set(res.sectores.data);
        if (res.motivos.success) this.listaMotivos.set(res.motivos.data);
        
        // 🚀 PASO 4: Una vez que las opciones del @for existen, inyectamos la data de la BD
        this.parcharDatosFormulario();
        this.cargandoCentros.set(false);
      },
      error: () => this.cargandoCentros.set(false)
    });
  }

  crearFormularioVacio(): void {
    this.expForm = this.fb.group({
      empresaInstitucion: ['', [Validators.required]],
      cargoPuesto: ['', [Validators.required]],
      fechaInicio: ['', [Validators.required]],
      fechaFin: [null],
      esSectorPublico: [false],
      esExperienciaEspecifica: [false],
      funcionesPrincipales: ['', [Validators.required, Validators.maxLength(1000)]],
      idSectorCat: ['', [Validators.required]],
      idRegimenCat: [{ value: '', disabled: true }, [Validators.required]], // Inicializa deshabilitado limpiamente
      idMotivoCambioCat: ['', [Validators.required]],
      remuneracionMensual: ['', [Validators.required, Validators.min(0)]]
    });
  }

  cargarCatalogosMaestros(): void {
    this.catalogoService.getValoresByCodigo('SEC').subscribe(res => {
      if (res.success) this.listaSectores.set(res.data);
    });

    this.catalogoService.getValoresByCodigo('MOTIVO_CAMBIO').subscribe(res => {
      if (res.success) this.listaMotivos.set(res.data);
    });
  }

  parcharDatosFormulario(): void {
    if (!this.isEdicion) return;
    const elem = this.data?.elemento;

    this.expForm.patchValue({
      empresaInstitucion: elem?.empresaInstitucion,
      cargoPuesto: elem?.cargoPuesto,
      fechaInicio: elem?.fechaInicio ? elem.fechaInicio.split('T')[0] : '',
      fechaFin: elem?.fechaFin ? elem.fechaFin.split('T')[0] : null,
      esSectorPublico: elem?.esSectorPublico || false,
      esExperienciaEspecifica: elem?.esExperienciaEspecifica || false,
      funcionesPrincipales: elem?.funcionesPrincipales,
      idSectorCat: elem?.idSectorCat,
      idMotivoCambioCat: elem?.idMotivoCambioCat,
      remuneracionMensual: elem?.remuneracionMensual ? this.formatearMoneda(elem.remuneracionMensual.toString()) : ''
    });

    // Carga síncrona del catálogo hijo de regímenes en base al sector recuperado
    if (elem?.idSectorCat) {
      this.cargarRegimenEdicionSincrono(elem.idSectorCat, elem.idRegimenCat);
    }
  }

  onRemuneracionInput(valor: string): void {
    if (!valor) return;

    let limpio = valor.replace(/[^0-9.]/g, '');

    const partes = limpio.split('.');
    if (partes.length > 2) {
      limpio = partes[0] + '.' + partes.slice(1).join('');
    }

    let entera = partes[0];
    const decimal = partes[1] !== undefined ? '.' + partes[1] : '';

    if (entera) {
      entera = Number(entera).toLocaleString('en-US'); 
    }

    this.expForm.get('remuneracionMensual')?.setValue(entera + decimal, { emitEvent: false });
  }

  onRemuneracionBlur(): void {
    const control = this.expForm.get('remuneracionMensual');
    if (!control || !control.value) return;

    const valorFormateado = this.formatearMoneda(control.value);
    control.setValue(valorFormateado, { emitEvent: false });
  }

  formatearMoneda(valor: string): string {
    const numeroPuro = parseFloat(valor.replace(/,/g, ''));
    if (isNaN(numeroPuro)) return '';

    return numeroPuro.toLocaleString('en-US', {
      minimumFractionDigits: 2,
      maximumFractionDigits: 2
    });
  }

  onSectorChange(idSector: any): void {
    this.expForm.get('idRegimenCat')?.setValue('');
    this.listaRegimenes.set([]);

    if (!idSector) {
      this.expForm.get('idRegimenCat')?.disable();
      return;
    }

    const idSectorNumerico = Number(idSector);
    const sectorElegido = this.listaSectores().find(s => Number(s.idValor) === idSectorNumerico);
    
    if (!sectorElegido) return;

    const esPub = sectorElegido.codigoValor === 'PUB';
    this.expForm.patchValue({ esSectorPublico: esPub });

    const nombreGrupoRegimen = esPub ? 'TIPO_REGIMEN_PUB' : 'TIPO_REGIMEN_PRI';

    this.catalogoService.getValoresByCodigo(nombreGrupoRegimen).subscribe({
      next: (res) => {
        if (res.success && Array.isArray(res.data)) {
          this.listaRegimenes.set(res.data);
          if (res.data.length > 0) {
            this.expForm.get('idRegimenCat')?.enable();
          } else {
            this.expForm.get('idRegimenCat')?.disable();
          }
        }
      }
    });
  }

  cargarRegimenEdicionSincrono(idSector: number, idRegimenGuardado: number): void {
    const sectorElegido = this.listaSectores().find(s => Number(s.idValor) === Number(idSector));
    if (!sectorElegido) return;

    const esPub = sectorElegido.codigoValor === 'PUB';
    const nombreGrupoRegimen = esPub ? 'TIPO_REGIMEN_PUB' : 'TIPO_REGIMEN_PRI';

    this.catalogoService.getValoresByCodigo(nombreGrupoRegimen).subscribe({
      next: (res) => {
        if (res.success && Array.isArray(res.data)) {
          this.listaRegimenes.set(res.data);
          
          // 🚀 BLINDAJE ANTI-NG0100: Seteamos el valor de manera secuencial y segura
          this.expForm.get('idRegimenCat')?.enable({ emitEvent: false });
          this.expForm.get('idRegimenCat')?.setValue(idRegimenGuardado, { emitEvent: false });
        }
      }
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
    
    const formValue = this.expForm.getRawValue();
    const idExperienciaActual = this.data?.elemento?.idExperiencia ?? this.data?.elemento?.IdExperiencia ?? 0;

    if (formValue.remuneracionMensual) {
      const valorLimpio = formValue.remuneracionMensual.toString().replace(/,/g, '');
      formValue.remuneracionMensual = parseFloat(valorLimpio);
    }

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
