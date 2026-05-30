using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Catalogos.Dtos;
using Application.Catalogos.Queries;
using Application.Common.Interfaces;
using MediatR;

namespace Application.Catalogos.Handlers
{
    public class GetCatalogoTiposQueryHandler : IRequestHandler<GetCatalogoTiposQuery, List<CatalogoTipoDto>>
    {
        private readonly ICatalogoQueryService _queryService;

        public GetCatalogoTiposQueryHandler(ICatalogoQueryService queryService)
        {
            _queryService = queryService;
        }

        public async Task<List<CatalogoTipoDto>> Handle(GetCatalogoTiposQuery request, CancellationToken cancellationToken)
        {
            return await _queryService.ObtenerTiposActivosAsync();
        }
    }
}
