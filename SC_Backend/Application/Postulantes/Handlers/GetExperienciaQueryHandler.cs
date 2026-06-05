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
    public class GetExperienciaQueryHandler : IRequestHandler<GetExperienciaLaboralQuery, List<PostulanteExperienciaDto>>
    {
        private readonly IPostulanteQueryService _queryService;

        public GetExperienciaQueryHandler(IPostulanteQueryService queryService)
        {
            _queryService = queryService;
        }

        public async Task<List<PostulanteExperienciaDto>> Handle(GetExperienciaLaboralQuery request, CancellationToken cancellationToken)
        {
            return await _queryService.ListarExperienciaAsync(request.IdPostulante);
        }
    }
}
