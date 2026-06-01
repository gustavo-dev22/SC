import { inject, Injectable } from '@angular/core';
import { CrudHttpService } from '../core/services/crud-http.service';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class PostulantePerfilService {
  private crudHttp = inject(CrudHttpService);

  getPerfil(id: number): Observable<any> {
    return this.crudHttp.get<any>(`postulanteperfil/${id}`);
  }

  updatePerfil(payload: any): Observable<any> {
    return this.crudHttp.put<any>('postulanteperfil', payload);
  }
}
