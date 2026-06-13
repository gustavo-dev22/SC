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
    public class ObtenerFirmaQueryHandler : IRequestHandler<ObtenerFirmaQuery, PostulanteFirmaDto?>
    {
        private readonly IPostulanteQueryService _queryService;
        public ObtenerFirmaQueryHandler(IPostulanteQueryService queryService) => _queryService = queryService;

        public async Task<PostulanteFirmaDto?> Handle(ObtenerFirmaQuery request, CancellationToken cancellationToken)
        {
            return await _queryService.ObtenerFirmaAsync(request.IdPostulante);
        }
    }
}
