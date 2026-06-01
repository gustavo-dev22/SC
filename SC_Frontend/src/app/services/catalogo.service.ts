import { inject, Injectable } from '@angular/core';
import { CrudHttpService } from '../core/services/crud-http.service'; // Ajusta la ruta relativa según tu estructura
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class CatalogoService {
  private crudHttp = inject(CrudHttpService);

  getValoresByTipo(idTipo: number, page: number, size: number): Observable<any> {
    return this.crudHttp.get<any>(`catalogo/valores/${idTipo}?pageNumber=${page}&pageSize=${size}`);
  }

  getValoresByCodigo(codigo: string): Observable<any> {
    return this.crudHttp.get<any>(`catalogo/valores-por-codigo/${codigo}`);
  }

  mantenimiento(payload: any): Observable<any> {
    return this.crudHttp.post<any>('catalogo/mantenimiento', payload);
  }

  mantenimientoTipo(payload: any): Observable<any> {
    return this.crudHttp.post<any>('catalogo/tipo-mantenimiento', payload);
  }

  getTipos(): Observable<any> {
    return this.crudHttp.get<any>('catalogo/tipos');
  }

  getCentrosEstudiosUnificados(filtro: string): Observable<any> {
    return this.crudHttp.get<any>(`catalogo/centros-estudios?query=${encodeURIComponent(filtro)}`);
  }
}