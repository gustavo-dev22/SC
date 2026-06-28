import { inject, Injectable } from '@angular/core';
import { CrudHttpService } from '../core/services/crud-http.service';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class DashboardAdminService {
  private crudHttp = inject(CrudHttpService);

  obtenerResumen(): Observable<any> {
    return this.crudHttp.get<any>('AdminSoporte/dashboard-summary');
  }
}