using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Parametros.Dtos;

namespace Application.Common.Interfaces
{
    public interface IParametroQueryService
    {
        Task<List<ParametroGlobalDto>> ObtenerParametrosAsync(string? codigo);
        Task<bool> ObtenerEstadoMantenimientoAsync();
    }
}
