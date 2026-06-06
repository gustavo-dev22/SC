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
    public class GetOfimaticaQueryHandler : IRequestHandler<GetOfimaticaQuery, List<PostulanteOfimaticaDto>>
    {
        private readonly IPostulanteQueryService _queryService;
        public GetOfimaticaQueryHandler(IPostulanteQueryService queryService) => _queryService = queryService;

        public async Task<List<PostulanteOfimaticaDto>> Handle(GetOfimaticaQuery request, CancellationToken cancellationToken)
        {
            return await _queryService.ListarOfimaticaAsync(request.IdPostulante);
        }
    }
}
