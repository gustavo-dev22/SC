using Application.Postulantes.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
