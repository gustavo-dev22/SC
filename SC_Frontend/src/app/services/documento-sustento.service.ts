import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { CrudHttpService } from '../core/services/crud-http.service';

@Injectable({
  providedIn: 'root'
})
export class DocumentoSustentoService {
  private crudHttp = inject(CrudHttpService);

  /**
   * Sube cualquier archivo de sustento al backend
   * @param controlador El nombre del controlador (ej. 'postulanteformacion' o 'postulantecertificacion')
   * @param idRegistro El ID de la entidad (idFormacion, idCertificacion, etc.)
   * @param nombreParamId El nombre exacto que espera el FromForm de .NET (idFormacion o idCertificacion)
   * @param file El documento PDF
   */
  subirPdf(controlador: string, idRegistro: number, nombreParamId: string, file: File): Observable<any> {
    const formData = new FormData();
    formData.append('archivo', file);
    formData.append(nombreParamId, idRegistro.toString());

    return this.crudHttp.post<any>(`${controlador}/subir-sustento`, formData);
  }

  /**
   * Elimina cualquier archivo de sustento del backend
   * @param controlador El nombre del controlador
   * @param idRegistro El ID de la entidad
   */
  eliminarPdf(controlador: string, idRegistro: number): Observable<any> {
    return this.crudHttp.delete<any>(`${controlador}/eliminar-sustento/${idRegistro}`);
  }
}