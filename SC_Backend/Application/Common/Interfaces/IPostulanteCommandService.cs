using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Auth.Dtos;
using Application.Postulantes.Commands;
using Application.Postulantes.Dtos;

namespace Application.Common.Interfaces
{
    public interface IPostulanteCommandService
    {
        Task<bool> RegistrarPostulanteAsync(RegistrarPostulanteCommand command, string computedHash);
        Task<bool> ActualizarPerfilAsync(ActualizarPerfilCommand command);
        Task<bool> MantenimientoFormacionAsync(MantenimientoFormacionCommand command);
        Task<bool> MantenimientoCertificacionAsync(MantenimientoCertificacionCommand command);
        Task<bool> MantenimientoExperienciaAsync(MantenimientoExperienciaCommand command);
        Task<bool> MantenimientoColegiaturaAsync(MantenimientoColegiaturaCommand command);
        Task<bool> MantenimientoIdiomaAsync(MantenimientoIdiomaCommand command);
        Task<bool> MantenimientoOfimaticaAsync(MantenimientoOfimaticaCommand command);
        Task<bool> MantenimientoRequisitoEspecialAsync(MantenimientoRequisitoEspecialCommand command);
        Task<bool> GuardarInfoAdicionalAsync(GuardarInfoAdicionalCommand command);
        Task<bool> GuardarFirmaAsync(int idPostulante, byte[] archivoBytes, string tipoMime);
        Task<DatosPostulanteTokenDto?> RegistrarTokenRecuperacionAsync(string numDocumento, string token);
        Task<bool> RestablecerPasswordAsync(string token, string nuevoPasswordHash);
        Task<bool> GuardarDeclaracionesAsync(int idPostulante, List<GuardarDeclaracionItemDto> declaraciones);
        Task<int> ObtenerTotalPostulacionesAnualAsync(int anio);
        Task<bool> InsertarPostulacionLocalAsync(int idPostulante, int idPlaza, int idEstadoCat, string codigoPostulacion);
    }
}
