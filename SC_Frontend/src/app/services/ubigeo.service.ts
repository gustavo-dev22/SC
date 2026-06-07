import { Injectable, inject } from '@angular/core';
import { CrudHttpService } from '../core/services/crud-http.service';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class UbigeoService {
  private crudHttp = inject(CrudHttpService);

  getDepartamentos(): Observable<any> { return this.crudHttp.get('ubigeo/departamentos'); }
  getProvincias(idDep: string): Observable<any> { return this.crudHttp.get(`ubigeo/provincias/${idDep}`); }
  getDistritos(idProv: string): Observable<any> { return this.crudHttp.get(`ubigeo/distritos/${idProv}`); }
}
