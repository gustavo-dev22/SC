import { inject, Injectable } from '@angular/core';
import { CrudHttpService } from '../core/services/crud-http.service';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class PostulanteExperienciaService {
  private crudHttp = inject(CrudHttpService);

  getExperiencias(idPostulante: number): Observable<any> {
    return this.crudHttp.get<any>(`postulanteexperiencia/postulante/${idPostulante}`);
  }

  mantenimiento(payload: any): Observable<any> {
    return this.crudHttp.post<any>('postulanteexperiencia/mantenimiento', payload);
  }
}
