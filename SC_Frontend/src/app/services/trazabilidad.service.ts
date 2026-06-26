import { inject, Injectable } from '@angular/core';
import { CrudHttpService } from '../core/services/crud-http.service';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class TrazabilidadService {
  private crudHttp = inject(CrudHttpService);

  consultarTrazabilidad(codigoExpediente: string): Observable<any> {
    return this.crudHttp.get<any>(`AdminPostulacion/trazabilidad?codigoExpediente=${codigoExpediente}`);
  }
}
