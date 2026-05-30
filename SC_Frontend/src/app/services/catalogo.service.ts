import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class CatalogoService {
  private http = inject(HttpClient);
  private baseUrl = environment.apiUrl;

  getValoresByTipo(idTipo: number, page: number, size: number): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/catalogo/valores/${idTipo}?pageNumber=${page}&pageSize=${size}`);
  }

  mantenimiento(payload: any): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/catalogo/mantenimiento`, payload);
  }

  mantenimientoTipo(payload: any): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/catalogo/tipo-mantenimiento`, payload);
  }

  getTipos(): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/catalogo/tipos`);
  }
}
