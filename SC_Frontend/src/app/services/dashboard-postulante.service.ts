import { inject, Injectable } from '@angular/core';
import { CrudHttpService } from '../core/services/crud-http.service';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class PostulanteDashboardService {
  private crudHttp = inject(CrudHttpService);

  obtenerResumen(idPostulante: number): Observable<any> {
    return this.crudHttp.get<any>(`PostulanteResumen/dashboard-summary?idPostulante=${idPostulante}`);
  }
}
