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
    public class GetMisPostulacionesQueryHandler : IRequestHandler<GetMisPostulacionesQuery, List<MisPostulacionesDto>>
    {
        private readonly IOportunidadesQueryService _queryService;

        public GetMisPostulacionesQueryHandler(IOportunidadesQueryService queryService)
        {
            _queryService = queryService;
        }

        public async Task<List<MisPostulacionesDto>> Handle(GetMisPostulacionesQuery request, CancellationToken cancellationToken)
        {
            return await _queryService.ObtenerMisPostulacionesAsync(request.IdPostulante);
        }
    }
}
