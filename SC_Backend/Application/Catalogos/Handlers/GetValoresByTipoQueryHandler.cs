using Application.Catalogos.Dtos;
using Application.Catalogos.Queries;
using Application.Common.Interfaces;
using MediatR;
using System.Data;

namespace Application.Catalogos.Handlers
{
    public class GetValoresByTipoQueryHandler : IRequestHandler<GetValoresByTipoQuery, List<CatalogoValorDto>>
    {
        private readonly ICatalogoQueryService _queryService;

        public GetValoresByTipoQueryHandler(ICatalogoQueryService queryService)
        {
            _queryService = queryService;
        }

        public async Task<List<CatalogoValorDto>> Handle(GetValoresByTipoQuery request, CancellationToken cancellationToken)
        {
            return await _queryService.ObtenerValoresByTipoAsync(request.IdTipo, request.PageNumber, request.PageSize);
        }
    }
}
