import { inject, Injectable } from '@angular/core';
import { CrudHttpService } from '../core/services/crud-http.service';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class OportunidadesService {
  private crudHttp = inject(CrudHttpService);

  buscarPlazasVacantes(idPostulante: number, search: string = '', page: number = 1, size: number = 10): Observable<any> {
    return this.crudHttp.get(`oportunidades/buscar-plazas/${idPostulante}?search=${search}&page=${page}&size=${size}`);
  }

  registrarPostulacion(idPostulante: number, idPlaza: number, fechaFinPlaza: Date | string, yaPostulo: boolean): Observable<any> {
    return this.crudHttp.post('oportunidades/registrar-postulacion', { idPostulante, idPlaza, fechaFinPlaza, yaPostulo });
  }

  obtenerMisPostulaciones(idPostulante: number): Observable<any> {
    return this.crudHttp.get(`oportunidades/mis-postulaciones/${idPostulante}`);
  }

  imprimirConstanciaReporte(idPostulacion: number): Observable<Blob> {
    // Apunta a: api/postulantePdfReporte/reporte-constancia/{id}
    return this.crudHttp.get<Blob>(`postulantePdfReporte/reporte-constancia/${idPostulacion}`, { responseType: 'blob' });
  }
}
