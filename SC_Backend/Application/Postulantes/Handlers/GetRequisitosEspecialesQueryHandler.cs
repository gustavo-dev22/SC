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
    public class GetRequisitosEspecialesQueryHandler : IRequestHandler<GetRequisitosEspecialesQuery, List<PostulanteRequisitoEspecialDto>>
    {
        private readonly IPostulanteQueryService _queryService;
        public GetRequisitosEspecialesQueryHandler(IPostulanteQueryService queryService) => _queryService = queryService;

        public async Task<List<PostulanteRequisitoEspecialDto>> Handle(GetRequisitosEspecialesQuery request, CancellationToken cancellationToken)
        {
            return await _queryService.ListarRequisitosEspecialesAsync(request.IdPostulante);
        }
    }
}
