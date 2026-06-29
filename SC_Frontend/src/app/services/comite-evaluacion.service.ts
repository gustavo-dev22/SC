import { inject, Injectable } from '@angular/core';
import { CrudHttpService } from '../core/services/crud-http.service';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class ComiteEvaluacionService {
  private crudHttp = inject(CrudHttpService);

  listarPlazasAsignadasComite(): Observable<any> {
    return this.crudHttp.get<any>('Oportunidades/buscar-plazas/0?search=&page=1&size=100');
  }

  listarInscritos(idPlaza?: number): Observable<any> {
    const url = idPlaza ? `ComiteEvaluador/expedientes-inscritos?idPlaza=${idPlaza}` : 'ComiteEvaluador/expedientes-inscritos';
    return this.crudHttp.get<any>(url);
  }

  evaluarExpediente(idPostulacion: number, aprobado: boolean, observacion: string): Observable<any> {
    return this.crudHttp.post<any>('ComiteEvaluador/evaluar-inicial', { idPostulacion, aprobado, observacion });
  }

  descargarActaInicialPdf(idPlaza: number): Observable<Blob> {
    return this.crudHttp.get(`ComiteEvaluador/exportar-acta-inicial/${idPlaza}`, { responseType: 'blob' });
  }

  listarCandidatosExamen(idPlaza: number): Observable<any> {
    return this.crudHttp.get<any>(`ComiteEvaluador/examen-conocimientos?idPlaza=${idPlaza}`);
  }

  registrarNotaExamen(payload: { idPostulacion: number, notaConocimientos: number }): Observable<any> {
    return this.crudHttp.post<any>('ComiteEvaluador/registrar-nota-examen', payload);
  }

  descargarActaConocimientosPdf(idPlaza: number): Observable<Blob> {
    return this.crudHttp.get(`ComiteEvaluador/exportar-acta-conocimientos/${idPlaza}`, { responseType: 'blob' });
  }
}
