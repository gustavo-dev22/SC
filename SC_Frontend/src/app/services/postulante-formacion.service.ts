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

  SubirPdfSustento(formData: FormData): Observable<any> {
    return this.crudHttp.post<any>('postulanteformacion/subir-sustento', formData);
  }

  EliminarPdfSustento(idFormacion: number): Observable<any> {
    return this.crudHttp.delete<any>(`postulanteformacion/eliminar-sustento/${idFormacion}`);
  }
}
