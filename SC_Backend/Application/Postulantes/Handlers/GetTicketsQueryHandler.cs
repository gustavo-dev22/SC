using Application.Common.Interfaces;
using Application.Postulantes.Dtos;
using Application.Postulantes.Queries;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Postulantes.Handlers
{
    public class GetTicketsQueryHandler : IRequestHandler<GetTicketsQuery, List<SoporteTicketDto>>
    {
        private readonly IPostulanteQueryService _queryService;

        public GetTicketsQueryHandler(IPostulanteQueryService queryService)
        {
            _queryService = queryService;
        }

        public async Task<List<SoporteTicketDto>> Handle(GetTicketsQuery request, CancellationToken cancellationToken)
        {
            return await _queryService.ObtenerTicketsPorPostulanteAsync(request.IdPostulante);
        }
    }
}
