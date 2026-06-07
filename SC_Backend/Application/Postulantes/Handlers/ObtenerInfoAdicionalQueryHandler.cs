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
    public class ObtenerInfoAdicionalQueryHandler : IRequestHandler<ObtenerInfoAdicionalQuery, InfoAdicionalDto?>
    {
        private readonly IPostulanteQueryService _queryService;

        public ObtenerInfoAdicionalQueryHandler(IPostulanteQueryService queryService)
        {
            _queryService = queryService;
        }

        public async Task<InfoAdicionalDto?> Handle(ObtenerInfoAdicionalQuery request, CancellationToken cancellationToken)
        {
            return await _queryService.ObtenerInfoAdicionalAsync(request.IdPostulante);
        }
    }
}
