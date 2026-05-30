import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class ParametroService {
  private http = inject(HttpClient);
  private baseUrl = `${environment.apiUrl}/parametro`;

  getParametros(): Observable<any> {
    return this.http.get<any>(this.baseUrl);
  }

  updateParametro(payload: { codigo: string, valor: string }): Observable<any> {
    return this.http.put<any>(this.baseUrl, payload);
  }
}
