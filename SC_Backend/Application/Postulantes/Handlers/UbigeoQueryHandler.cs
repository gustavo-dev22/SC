using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Dtos;
using Application.Common.Interfaces;
using Application.Postulantes.Queries;
using MediatR;

namespace Application.Postulantes.Handlers
{
    public class GetDepartamentosQueryHandler : IRequestHandler<GetDepartamentosQuery, List<UbigeoDto>>
    {
        private readonly IPostulanteQueryService _queryService;
        public GetDepartamentosQueryHandler(IPostulanteQueryService queryService) => _queryService = queryService;

        public async Task<List<UbigeoDto>> Handle(GetDepartamentosQuery request, CancellationToken cancellationToken)
        {
            return await _queryService.ObtenerDepartamentosAsync();
        }
    }

    public class GetProvinciasQueryHandler : IRequestHandler<GetProvinciasQuery, List<UbigeoDto>>
    {
        private readonly IPostulanteQueryService _queryService;
        public GetProvinciasQueryHandler(IPostulanteQueryService queryService) => _queryService = queryService;

        public async Task<List<UbigeoDto>> Handle(GetProvinciasQuery request, CancellationToken cancellationToken)
        {
            return await _queryService.ObtenerProvinciasAsync(request.IdDepartamento);
        }
    }

    public class GetDistritosQueryHandler : IRequestHandler<GetDistritosQuery, List<UbigeoDto>>
    {
        private readonly IPostulanteQueryService _queryService;
        public GetDistritosQueryHandler(IPostulanteQueryService queryService) => _queryService = queryService;

        public async Task<List<UbigeoDto>> Handle(GetDistritosQuery request, CancellationToken cancellationToken)
        {
            return await _queryService.ObtenerDistritosAsync(request.IdProvincia);
        }
    }
}
