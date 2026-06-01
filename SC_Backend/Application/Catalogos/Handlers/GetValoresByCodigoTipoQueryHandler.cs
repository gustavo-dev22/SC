using Application.Catalogos.Dtos;
using Application.Catalogos.Queries;
using Application.Common.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Catalogos.Handlers
{
    public class GetValoresByCodigoTipoQueryHandler : IRequestHandler<GetValoresByCodigoTipoQuery, List<CatalogoValorDto>>
    {
        private readonly ICatalogoQueryService _catalogoQueryService;

        public GetValoresByCodigoTipoQueryHandler(ICatalogoQueryService catalogoQueryService)
        {
            _catalogoQueryService = catalogoQueryService;
        }

        public async Task<List<CatalogoValorDto>> Handle(GetValoresByCodigoTipoQuery request, CancellationToken cancellationToken)
        {
            return await _catalogoQueryService.ListarValoresByCodigoTipoAsync(request.Codigo);
        }
    }
}
