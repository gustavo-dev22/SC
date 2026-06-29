using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Postulantes.Queries;
using MediatR;

namespace Application.Postulantes.Handlers
{
    public class GetEstadoPostulacionActualQueryHandler : IRequestHandler<GetEstadoPostulacionActualQuery, int?>
    {
        private readonly IPostulanteQueryService _queryService;

        public GetEstadoPostulacionActualQueryHandler(IPostulanteQueryService queryService)
        {
            _queryService = queryService;
        }

        public async Task<int?> Handle(GetEstadoPostulacionActualQuery request, CancellationToken cancellationToken)
        {
            return await _queryService.ObtenerEstadoVigentePorUsuarioAsync(request.IdUsuario);
        }
    }
}
