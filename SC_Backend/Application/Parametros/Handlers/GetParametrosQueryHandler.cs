using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Parametros.Dtos;
using Application.Parametros.Queries;
using MediatR;

namespace Application.Parametros.Handlers
{
    public class GetParametrosQueryHandler : IRequestHandler<GetParametrosQuery, List<ParametroGlobalDto>>
    {
        private readonly IParametroQueryService _queryService;
        public GetParametrosQueryHandler(IParametroQueryService srv) => _queryService = srv;

        public async Task<List<ParametroGlobalDto>> Handle(GetParametrosQuery request, CancellationToken token)
        {
            return await _queryService.ObtenerParametrosAsync();
        }
    }
}
