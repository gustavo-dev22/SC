using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Oportunidades.Dtos;
using MediatR;

namespace Application.Oportunidades.Queries
{
    public record ListarPlazasVacantesQuery(
        int IdPostulante,
        string? Search,
        int Page,
        int Size
    ) : IRequest<PaginatedSpcResponseDto<PlazaVacanteDto>>;
}
