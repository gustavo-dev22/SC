import { inject, Injectable } from '@angular/core';
import { CrudHttpService } from '../core/services/crud-http.service';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class AdminSoporteService {
  private crudHttp = inject(CrudHttpService);

  obtenerBandeja(idEstado?: number, busqueda?: string): Observable<any> {
    let url = 'AdminSoporte/bandeja?';
    if (idEstado) url += `idEstado=${idEstado}&`;
    if (busqueda) url += `busqueda=${encodeURIComponent(busqueda)}`;
    return this.crudHttp.get<any>(url);
  }

  atenderTicket(command: any): Observable<any> {
    return this.crudHttp.post<any>('AdminSoporte/atender', command);
  }
}
