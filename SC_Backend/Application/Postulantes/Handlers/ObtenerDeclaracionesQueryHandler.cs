using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Postulantes.Dtos;
using Application.Postulantes.Queries;
using MediatR;

namespace Application.Postulantes.Handlers
{
    public class ObtenerDeclaracionesQueryHandler : IRequestHandler<ObtenerDeclaracionesQuery, List<PostulanteDeclaracionDto>>
    {
        private readonly IPostulanteQueryService _queryService;
        public ObtenerDeclaracionesQueryHandler(IPostulanteQueryService queryService) => _queryService = queryService;

        public async Task<List<PostulanteDeclaracionDto>> Handle(ObtenerDeclaracionesQuery request, CancellationToken cancellationToken)
        {
            return await _queryService.ListarDeclaracionesAsync(request.IdPostulante, request.IdTipo);
        }
    }
}
