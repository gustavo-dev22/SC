import { CommonModule } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { PostulantePerfilService } from '../../../services/postulante-perfil.service';
import { CatalogoService } from '../../../services/catalogo.service';
import { AlertService } from '../../../shared/services/alert.service';
import { UbigeoService } from '../../../services/ubigeo.service';

@Component({
  selector: 'app-datos-personales',
  imports: [
    CommonModule, ReactiveFormsModule, MatCardModule, MatFormFieldModule,
    MatInputModule, MatSelectModule, MatButtonModule, MatIconModule, MatProgressSpinnerModule
  ],
  templateUrl: './datos-personales.html',
  styleUrl: './datos-personales.css',
})
export class DatosPersonales implements OnInit {
  private fb = inject(FormBuilder);
  private perfilService = inject(PostulantePerfilService);
  private catalogoService = inject(CatalogoService);
  private alertService = inject(AlertService);
  private ubigeoService = inject(UbigeoService);

  public perfilForm!: FormGroup;
  public cargando = signal<boolean>(false);
  public listaSexo = signal<any[]>([]); 
  private idPostulanteLogueado!: number;

  public listaTipoVia = signal<any[]>([]);
  public listaTipoZona = signal<any[]>([]);
  public departamentos = signal<any[]>([]);
  public provincias = signal<any[]>([]);
  public distritos = signal<any[]>([]);

  ngOnInit(): void {
    this.inicializarFormulario();
    this.recuperarIdSesion();
    this.cargarCatálogos();
  }

  inicializarFormulario(): void {
    this.perfilForm = this.fb.group({
      numDocumento: [{ value: '', disabled: true }],
      nombres: [{ value: '', disabled: true }],
      apellidoPaterno: [{ value: '', disabled: true }],
      apellidoMaterno: [{ value: '', disabled: true }],
      correo: [{ value: '', disabled: true }],
      telefono: ['', [Validators.required, Validators.pattern('^[0-9]{9}$')]],
      fechaNacimiento: ['', [Validators.required]],
      idSexoCat: [0, [Validators.required, Validators.min(1)]],
      direccion: ['', [Validators.required, Validators.minLength(10)]],
      idTipoViaCat: ['', [Validators.required]],
      numeroVia: [''],
      numeroDepto: [''],
      interior: [''],
      manzana: [''],
      lote: [''],
      kilometro: [''],
      blockEdificio: [''],
      etapa: [''],
      idTipoZonaCat: ['', [Validators.required]],
      nombreZona: ['', [Validators.required]],

      idDepartamento: ['', [Validators.required]],
      idProvincia: ['', [Validators.required]],
      idUbigeoDistrito: ['', [Validators.required]],
      referenciaDireccion: ['']
    });
  }

  recuperarIdSesion(): void {
    const rawProfile = sessionStorage.getItem('user_profile');
    if (rawProfile) {
      const profile = JSON.parse(rawProfile);
      
      if (profile.token) {
        try {
          const decodedToken = atob(profile.token);
          const tokenParts = decodedToken.split('-');
          this.idPostulanteLogueado = Number(tokenParts[1]);
          this.cargarDatosPerfil();
        } catch (error) {
          console.error('Error al descifrar el token del postulante:', error);
          this.idPostulanteLogueado = 1;
          this.cargarDatosPerfil();
        }
      }
    }
  }

  cargarCatálogos(): void {
    this.catalogoService.getValoresByCodigo('SEXO').subscribe({
      next: (res) => { 
        if (res.success) {
          this.listaSexo.set(res.data); 
        } 
      }
    });

    this.catalogoService.getValoresByCodigo('TIPO_VIA').subscribe(res => this.listaTipoVia.set(res.data));
    this.catalogoService.getValoresByCodigo('TIPO_ZONA').subscribe(res => this.listaTipoZona.set(res.data));
    
    this.ubigeoService.getDepartamentos().subscribe(res => {
      if (res.success) this.departamentos.set(res.data);
    });
  }

  onDepartamentoChange(idDep: string): void {
    this.perfilForm.patchValue({ idProvincia: '', idUbigeoDistrito: '' }); // Limpieza inmediata
    this.provincias.set([]);
    this.distritos.set([]);

    if (!idDep) return;
    this.ubigeoService.getProvincias(idDep).subscribe(res => {
      if (res.success) this.provincias.set(res.data);
    });
  }

  onProvinciaChange(idProv: string): void {
    this.perfilForm.patchValue({ idUbigeoDistrito: '' }); // Limpieza inmediata
    this.distritos.set([]);

    if (!idProv) return;
    this.ubigeoService.getDistritos(idProv).subscribe(res => {
      if (res.success) this.distritos.set(res.data);
    });
  }

  cargarDatosPerfil(): void {
    this.cargando.set(true);
    this.perfilService.getPerfil(this.idPostulanteLogueado).subscribe({
      next: (res) => {
        if (res.success && res.data) {
          const p = res.data;

          if (p.fechaNacimiento) {
            p.fechaNacimiento = p.fechaNacimiento.split('T')[0];
          }
          this.perfilForm.patchValue(p);

          if (p.idDepartamento) {
            // A. Pintamos el departamento guardado
            this.perfilForm.get('idDepartamento')?.setValue(p.idDepartamento, { emitEvent: false });

            // B. Cargamos de forma síncrona/secuencial las Provincias de ese departamento
            this.ubigeoService.getProvincias(p.idDepartamento).subscribe(resProv => {
              if (resProv.success) {
                this.provincias.set(resProv.data);
                
                // C. Una vez cargada la lista de provincias, seleccionamos la provincia guardada
                this.perfilForm.get('idProvincia')?.setValue(p.idProvincia, { emitEvent: false });

                // D. Cargamos de forma secuencial los Distritos de esa provincia
                this.ubigeoService.getDistritos(p.idProvincia).subscribe(resDist => {
                  if (resDist.success) {
                    this.distritos.set(resDist.data);
                    
                    // E. Finalmente, autoseleccionamos el distrito del postulante
                    this.perfilForm.get('idUbigeoDistrito')?.setValue(p.idUbigeoDistrito, { emitEvent: false });
                  }
                });
              }
            });
        }
        }
        this.cargando.set(false);
      },
      error: () => this.cargando.set(false)
    });
  }

  onSubmit(): void {
    if (this.perfilForm.invalid) {
      this.perfilForm.markAllAsTouched();
      return;
    }

    this.cargando.set(true);

    const formValues = this.perfilForm.getRawValue();
    
    const payload = {
      idPostulante: this.idPostulanteLogueado,
      telefono: formValues.telefono,
      fechaNacimiento: formValues.fechaNacimiento,
      idSexoCat: formValues.idSexoCat,
      direccion: formValues.direccion,
      
      // Configuración estructural de la dirección (Campos obligatorios y opcionales)
      idTipoViaCat: formValues.idTipoViaCat,
      numeroVia: formValues.numeroVia,
      numeroDepto: formValues.numeroDepto,
      interior: formValues.interior,
      manzana: formValues.manzana,
      lote: formValues.lote,
      kilometro: formValues.kilometro,
      blockEdificio: formValues.blockEdificio,
      etapa: formValues.etapa,
      idTipoZonaCat: formValues.idTipoZonaCat,
      nombreZona: formValues.nombreZona,
      
      // Ubigeo jerárquico (Se envía solo el distrito final que contiene toda la cadena INEI)
      idUbigeoDistrito: formValues.idUbigeoDistrito,
      referenciaDireccion: formValues.referenciaDireccion
    };

    this.perfilService.updatePerfil(payload).subscribe({
      next: (res) => {
        if (res.success) {
          this.alertService.exito('¡Actualizado!', res.message || 'Los datos de su perfil se actualizaron correctamente.');
          this.cargarDatosPerfil();
        } else {
          this.alertService.advertencia('Atención', res.message || 'No se pudo completar la actualización.');
          this.cargando.set(false);
        }
      },
      error: (err) => {
        this.cargando.set(false);
        this.alertService.error('Error del Sistema', 'Ocurrió un problema inesperado al comunicar con el servidor. Inténtelo nuevamente.');
      }
    });
  }
}
