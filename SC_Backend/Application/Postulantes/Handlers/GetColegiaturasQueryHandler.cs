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
    public class GetColegiaturasQueryHandler : IRequestHandler<GetColegiaturasQuery, List<PostulanteColegiaturaDto>>
    {
        private readonly IPostulanteQueryService _queryService;

        public GetColegiaturasQueryHandler(IPostulanteQueryService queryService)
        {
            _queryService = queryService;
        }

        public async Task<List<PostulanteColegiaturaDto>> Handle(GetColegiaturasQuery request, CancellationToken cancellationToken)
        {
            return await _queryService.ListarColegiaturasAsync(request.IdPostulante);
        }
    }
}
