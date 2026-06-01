import { inject, Injectable } from '@angular/core';
import { CrudHttpService } from '../core/services/crud-http.service';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class PostulanteFormacionService {
  private crudHttp = inject(CrudHttpService);

  getFormacion(idPostulante: number): Observable<any> {
    return this.crudHttp.get<any>(`postulanteformacion/postulante/${idPostulante}`);
  }

  mantenimiento(payload: any): Observable<any> {
    return this.crudHttp.post<any>('postulanteformacion/mantenimiento', payload);
  }
}
