import { inject, Injectable, signal } from '@angular/core';
import { CrudHttpService } from '../core/services/crud-http.service';
import { Observable, tap } from 'rxjs';
import { OportunidadesService } from './oportunidades.service';

@Injectable({ providedIn: 'root' })
export class PostulacionService {
  private crudHttp = inject(CrudHttpService);
  private _oportunidadesService = inject(OportunidadesService);
  
  public estadoPostulacion = signal<number | null>(null);
  public misPostulacionesActive = signal<any[]>([]);
  public plazaContextoSeleccionada = signal<number | null>(null);

  public cargarContextoPostulaciones(idPostulante: number): Observable<any> {
    return this._oportunidadesService.obtenerMisPostulaciones(idPostulante).pipe(
      tap(res => {
        if (res.success && res.data) {
          this.misPostulacionesActive.set(res.data);

          if (res.data.length > 0 && !this.plazaContextoSeleccionada()) {
            const primeraPlazaId = res.data[0].idPlaza;
            this.cambiarContextoPlaza(primeraPlazaId);
          }
        }
      })
    );
  }

  public cambiarContextoPlaza(idPlaza: number): void {
    this.plazaContextoSeleccionada.set(idPlaza);
    this.consultarEstadoPostulacion(idPlaza).subscribe();
  }

  public consultarEstadoPostulacion(idPlaza?: number | null): Observable<any> {
    const url = idPlaza 
      ? `postulanteresumen/estado-actual?idPlaza=${idPlaza}`
      : `postulanteresumen/estado-actual`;

    return this.crudHttp.get<any>(url).pipe(
      tap(res => {
        if (res.success) {
          this.estadoPostulacion.set(res.data);
        }
      })
    );
  }
}