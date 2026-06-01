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

  getParametros(): Observable<any> {
    return this.crudHttp.get<any>('parametro');
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
