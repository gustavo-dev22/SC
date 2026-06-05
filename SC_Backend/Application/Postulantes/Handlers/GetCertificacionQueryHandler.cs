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
    public class GetCertificacionQueryHandler : IRequestHandler<GetCertificacionesQuery, List<PostulanteCertificacionDto>>
    {
        private readonly IPostulanteQueryService _queryService;

        public GetCertificacionQueryHandler(IPostulanteQueryService queryService)
        {
            _queryService = queryService;
        }

        public async Task<List<PostulanteCertificacionDto>> Handle(GetCertificacionesQuery request, CancellationToken cancellationToken)
        {
            return await _queryService.ListarCertificacionesAsync(request.IdPostulante);
        }
    }
}
