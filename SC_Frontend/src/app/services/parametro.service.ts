import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';
import { CrudHttpService } from '../core/services/crud-http.service';

@Injectable({
  providedIn: 'root',
})
export class ParametroService {
  private crudHttp = inject(CrudHttpService);

  getParametros(codigo?: string): Observable<any> {
    const url = codigo ? `parametro?codigo=${codigo}` : 'parametro';
    return this.crudHttp.get<any>(url);
  }

  updateParametro(payload: { 
    accion: string, 
    codigo: string, 
    nombre: string, 
    valor: string, 
    descripcion: string, 
    categoria: string 
  }): Observable<any> {
    return this.crudHttp.post<any>('parametro/mantenimiento', payload);
  }
}
