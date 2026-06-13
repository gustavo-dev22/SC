import { inject, Injectable } from '@angular/core';
import { CrudHttpService } from '../core/services/crud-http.service';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class PostulanteFirmaService {
  private crudHttp = inject(CrudHttpService);

  getFirma(idPostulante: number): Observable<any> {
    return this.crudHttp.get(`postulanteFirma/firma/${idPostulante}`);
  }

  subirFirma(idPostulante: number, archivo: File): Observable<any> {
    const formData = new FormData();
    formData.append('idPostulante', idPostulante.toString());
    formData.append('archivo', archivo);

    return this.crudHttp.post('postulanteFirma/firma/subir', formData);
  }
}
