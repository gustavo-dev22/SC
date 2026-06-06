import { Injectable, inject } from '@angular/core';
import { CrudHttpService } from '../core/services/crud-http.service';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class PostulanteOfimaticaService {
  private crudHttp = inject(CrudHttpService);

  getOfimatica(idPostulante: number): Observable<any> {
    return this.crudHttp.get<any>(`postulanteofimatica/postulante/${idPostulante}`);
  }

  mantenimiento(payload: any): Observable<any> {
    return this.crudHttp.post<any>('postulanteofimatica/mantenimiento', payload);
  }
}
