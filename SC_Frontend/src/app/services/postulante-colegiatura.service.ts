import { Injectable, inject } from '@angular/core';
import { CrudHttpService } from '../core/services/crud-http.service';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class PostulanteColegiaturaService {
  private crudHttp = inject(CrudHttpService);

  getColegiaturas(idPostulante: number): Observable<any> {
    return this.crudHttp.get<any>(`postulantecolegiatura/postulante/${idPostulante}`);
  }

  mantenimiento(payload: any): Observable<any> {
    return this.crudHttp.post<any>('postulantecolegiatura/mantenimiento', payload);
  }
}
