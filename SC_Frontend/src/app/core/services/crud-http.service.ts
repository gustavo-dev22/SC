import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class CrudHttpService {
  private http = inject(HttpClient);
  private apiUrl = environment.apiUrl;

  // GET Genérico
  public get<T>(endpoint: string): Observable<T> {
    return this.http.get<T>(`${this.apiUrl}/${endpoint}`);
  }

  // POST Genérico
  public post<T>(endpoint: string, payload: any): Observable<T> {
    return this.http.post<T>(`${this.apiUrl}/${endpoint}`, payload);
  }

  // PUT Genérico
  public put<T>(endpoint: string, payload: any): Observable<T> {
    return this.http.put<T>(`${this.apiUrl}/${endpoint}`, payload);
  }

  // DELETE Genérico
  public delete<T>(endpoint: string): Observable<T> {
    return this.http.delete<T>(`${this.apiUrl}/${endpoint}`);
  }
}