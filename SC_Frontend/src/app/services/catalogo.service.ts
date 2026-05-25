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

  getValoresByTipo(idTipo: number): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/catalogo/valores/${idTipo}`);
  }

  mantenimiento(payload: any): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/catalogo/mantenimiento`, payload);
  }
}
