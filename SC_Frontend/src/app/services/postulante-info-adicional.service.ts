import { inject, Injectable } from '@angular/core';
import { CrudHttpService } from '../core/services/crud-http.service';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class PostulanteInfoAdicionalService {
  private crudHttp = inject(CrudHttpService);

  getInfoAdicional(idPostulante: number): Observable<any> {
    return this.crudHttp.get(`postulanteInfoAdicional/${idPostulante}`);
  }

  guardarInfoAdicional(payload: any): Observable<any> {
    return this.crudHttp.post('postulanteInfoAdicional/postulante/guardar', payload);
  }
}
