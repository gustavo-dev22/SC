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
    public class GetIdiomasQueryHandler : IRequestHandler<GetIdiomasQuery, List<PostulanteIdiomaDto>>
    {
        private readonly IPostulanteQueryService _queryService;
        public GetIdiomasQueryHandler(IPostulanteQueryService queryService) => _queryService = queryService;

        public async Task<List<PostulanteIdiomaDto>> Handle(GetIdiomasQuery request, CancellationToken cancellationToken)
        {
            return await _queryService.ListarIdiomasAsync(request.IdPostulante);
        }
    }
}
