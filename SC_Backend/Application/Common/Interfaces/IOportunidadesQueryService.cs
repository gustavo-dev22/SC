using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Oportunidades.Dtos;

namespace Application.Common.Interfaces
{
    public interface IOportunidadesQueryService
    {
        Task<PaginatedSpcResponseDto<PlazaVacanteDto>> ObtenerPlazasDisponiblesAsync(int idPostulante, string? search, int page, int size);
        Task<List<MisPostulacionesDto>> ObtenerMisPostulacionesAsync(int idPostulante);
        Task<MisPostulacionesDto?> ObtenerPostulacionPorIdAsync(int idPostulacion);
    }
}
