import { Injectable, inject, signal } from '@angular/core';
import { CrudHttpService } from '../core/services/crud-http.service';
import { Observable, tap } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class PostulanteResumenService {
  private crudHttp = inject(CrudHttpService);
  public estadoPostulacion = signal<number | null>(null);

  getAvanceCurriculum(idPostulante: number): Observable<any> {
    return this.crudHttp.get<any>(`postulanteresumen/avance/${idPostulante}`);
  }

  consultarEstadoPostulacion(): Observable<any> {
    return this.crudHttp.get<any>('postulanteresumen/estado-actual').pipe(
      tap(res => {
        if (res.success) {
          console.log('%c🔍 DEBUG POSTULACIÓN:', 'color: #2a5298; font-weight: bold;', res.data);
          // Guardamos el estado globalmente en la Signal
          this.estadoPostulacion.set(res.data);
        }
      })
    );
  }
}