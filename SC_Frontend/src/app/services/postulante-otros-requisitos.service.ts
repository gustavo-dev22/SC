import { Injectable, inject } from '@angular/core';
import { CrudHttpService } from '../core/services/crud-http.service';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class PostulanteOtrosRequisitosService {
  private crudHttp = inject(CrudHttpService);

  getRequisitos(idPostulante: number): Observable<any> {
    return this.crudHttp.get<any>(`postulanteotrosrequisitos/postulante/${idPostulante}`);
  }

  mantenimiento(payload: any): Observable<any> {
    return this.crudHttp.post<any>('postulanteotrosrequisitos/mantenimiento', payload);
  }
}