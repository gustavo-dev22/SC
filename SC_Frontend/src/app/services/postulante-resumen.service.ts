import { Injectable, inject } from '@angular/core';
import { CrudHttpService } from '../core/services/crud-http.service';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class PostulanteResumenService {
  private crudHttp = inject(CrudHttpService);

  getAvanceCurriculum(idPostulante: number): Observable<any> {
    return this.crudHttp.get<any>(`postulanteresumen/avance/${idPostulante}`);
  }
}