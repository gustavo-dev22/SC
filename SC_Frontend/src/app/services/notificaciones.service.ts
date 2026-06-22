import { inject, Injectable } from '@angular/core';
import { CrudHttpService } from '../core/services/crud-http.service';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class NotificacionesService {
  private crudHttp = inject(CrudHttpService);

  listarNotificaciones(idPostulante: number): Observable<any> {
    return this.crudHttp.get<any>(`PostulanteNotificaciones/${idPostulante}`);
  }

  marcarComoLeida(idNotificacion: number): Observable<any> {
    return this.crudHttp.put<any>('PostulanteNotificaciones/marcar-leida', { idNotificacion });
  }
}
