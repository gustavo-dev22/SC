import { inject, Injectable } from '@angular/core';
import { CrudHttpService } from '../core/services/crud-http.service';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class SoporteService {
  private crudHttp = inject(CrudHttpService);

  listarTickets(idPostulante: number): Observable<any> {
    return this.crudHttp.get<any>(`Soporte/tickets/${idPostulante}`);
  }

  enviarTicket(command: any): Observable<any> {
    return this.crudHttp.post<any>('Soporte/registrar-ticket', command);
  }
}
