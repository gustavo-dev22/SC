using Application.Common.Interfaces;
using Application.Parametros.Queries;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Parametros.Handlers
{
    public class GetMantenimientoPortalQueryHandler : IRequestHandler<GetMantenimientoPortalQuery, bool>
    {
        private readonly IParametroQueryService _queryService;

        public GetMantenimientoPortalQueryHandler(IParametroQueryService queryService)
        {
            _queryService = queryService;
        }

        public async Task<bool> Handle(GetMantenimientoPortalQuery request, CancellationToken cancellationToken)
        {
            return await _queryService.ObtenerEstadoMantenimientoAsync();
        }
    }
}
