import { inject, Injectable } from '@angular/core';
import { CrudHttpService } from '../core/services/crud-http.service';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class PostulanteCertificacionService {
  private crudHttp = inject(CrudHttpService);

  getCertificaciones(idPostulante: number): Observable<any> {
    return this.crudHttp.get<any>(`postulantecertificacion/postulante/${idPostulante}`);
  }

  mantenimiento(payload: any): Observable<any> {
    return this.crudHttp.post<any>('postulantecertificacion/mantenimiento', payload);
  }
}
