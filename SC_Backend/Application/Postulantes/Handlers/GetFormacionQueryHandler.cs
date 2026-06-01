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
    public class GetFormacionQueryHandler : IRequestHandler<GetFormacionQuery, List<PostulanteFormacionDto>>
    {
        private readonly IPostulanteQueryService _queryService;

        public GetFormacionQueryHandler(IPostulanteQueryService queryService)
        {
            _queryService = queryService;
        }

        public async Task<List<PostulanteFormacionDto>> Handle(GetFormacionQuery request, CancellationToken cancellationToken)
        {
            return await _queryService.ListarFormacionAsync(request.IdPostulante);
        }
    }
}
