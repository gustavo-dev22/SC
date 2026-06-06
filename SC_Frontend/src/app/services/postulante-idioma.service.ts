import { Injectable, inject } from '@angular/core';
import { CrudHttpService } from '../core/services/crud-http.service';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class PostulanteIdiomaService {
  private crudHttp = inject(CrudHttpService);

  getIdiomas(idPostulante: number): Observable<any> {
    return this.crudHttp.get<any>(`postulanteidioma/postulante/${idPostulante}`);
  }

  mantenimiento(payload: any): Observable<any> {
    return this.crudHttp.post<any>('postulanteidioma/mantenimiento', payload);
  }
}