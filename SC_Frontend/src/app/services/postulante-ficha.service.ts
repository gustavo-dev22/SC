import { inject, Injectable } from '@angular/core';
import { CrudHttpService } from '../core/services/crud-http.service';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class PostulanteFichaService {
  private crudHttp = inject(CrudHttpService);

  imprimirFichaReporte(idPostulante: number): Observable<Blob> {
    // Pasamos { responseType: 'blob' } para capturar el flujo de memoria binario directo
    return this.crudHttp.get<Blob>(`postulantePdfReporte/reporte-ficha/${idPostulante}`, { responseType: 'blob' });
  }
}
