import { inject, Injectable } from '@angular/core';
import { CrudHttpService } from '../core/services/crud-http.service';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class AuditoriaService {
  private crudHttp = inject(CrudHttpService);

  listarLogs(operacion?: string, fechaInicio?: string, fechaFin?: string): Observable<any> {
    let url = 'AdminSoporte/logs-auditoria?';
    if (operacion) url += `operacion=${operacion}&`;
    if (fechaInicio) url += `fechaInicio=${fechaInicio}&`;
    if (fechaFin) url += `fechaFin=${fechaFin}`;
    return this.crudHttp.get<any>(url);
  }
}
