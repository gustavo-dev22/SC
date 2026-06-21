using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Oportunidades.Dtos;
using Application.Oportunidades.Queries;
using MediatR;

namespace Application.Oportunidades.Handlers
{
    public class ListarPlazasVacantesQueryHandler : IRequestHandler<ListarPlazasVacantesQuery, PaginatedSpcResponseDto<PlazaVacanteDto>>
    {
        private readonly IOportunidadesQueryService _queryService;

        public ListarPlazasVacantesQueryHandler(IOportunidadesQueryService queryService)
        {
            _queryService = queryService;
        }

        public async Task<PaginatedSpcResponseDto<PlazaVacanteDto>> Handle(ListarPlazasVacantesQuery request, CancellationToken cancellationToken)
        {
            return await _queryService.ObtenerPlazasDisponiblesAsync(request.IdPostulante, request.Search, request.Page, request.Size);
        }
    }
}
