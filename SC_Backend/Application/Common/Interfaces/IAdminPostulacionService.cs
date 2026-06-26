using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Admin.Dtos;

namespace Application.Common.Interfaces
{
    public interface IAdminPostulacionService
    {
        Task<List<TrazabilidadExpedienteDto>> ObtenerTrazabilidadPorExpedienteAsync(string codigoExpediente);
    }
}
