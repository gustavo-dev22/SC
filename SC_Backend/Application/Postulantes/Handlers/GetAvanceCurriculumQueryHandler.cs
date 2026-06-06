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
    public class GetAvanceCurriculumQueryHandler : IRequestHandler<GetAvanceCurriculumQuery, AvanceCurriculumDto>
    {
        private readonly IPostulanteQueryService _queryService;
        public GetAvanceCurriculumQueryHandler(IPostulanteQueryService queryService) => _queryService = queryService;

        public async Task<AvanceCurriculumDto> Handle(GetAvanceCurriculumQuery request, CancellationToken cancellationToken)
        {
            return await _queryService.ObtenerAvanceCurriculumAsync(request.IdPostulante);
        }
    }
}
