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
    public class GetPerfilPostulanteQueryHandler : IRequestHandler<GetPerfilPostulanteQuery, PerfilPostulanteDto?>
    {
        private readonly IPostulanteQueryService _queryService;
        public GetPerfilPostulanteQueryHandler(IPostulanteQueryService srv) => _queryService = srv;

        public async Task<PerfilPostulanteDto?> Handle(GetPerfilPostulanteQuery request, CancellationToken token)
        {
            return await _queryService.ObtenerPerfilByIdAsync(request.IdPostulante);
        }
    }
}
