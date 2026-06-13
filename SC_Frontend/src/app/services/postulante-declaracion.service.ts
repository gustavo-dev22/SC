import { inject, Injectable } from '@angular/core';
import { CrudHttpService } from '../core/services/crud-http.service';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class PostulanteDeclaracionService {
  private crudHttp = inject(CrudHttpService);

  getDeclaraciones(idPostulante: number, idTipo: number): Observable<any[]> {
    return this.crudHttp.get(`postulanteDeclaracion/listar/${idPostulante}/${idTipo}`);
  }

  guardarDeclaraciones(idPostulante: number, declaraciones: any[]): Observable<any> {
    return this.crudHttp.post('postulanteDeclaracion/guardar', { idPostulante, declaraciones });
  }
}
