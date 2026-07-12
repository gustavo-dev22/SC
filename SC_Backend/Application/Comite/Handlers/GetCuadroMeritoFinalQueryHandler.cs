using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Comite.Dtos;
using Application.Comite.Queries;
using Application.Common.Interfaces;
using MediatR;

namespace Application.Comite.Handlers
{
    public class GetCuadroMeritoFinalQueryHandler : IRequestHandler<GetCuadroMeritoFinalQuery, List<CuadroMeritoFinalDto>>
    {
        private readonly IComiteEvaluadorService _comiteService;
        public GetCuadroMeritoFinalQueryHandler(IComiteEvaluadorService comiteService) => _comiteService = comiteService;

        public async Task<List<CuadroMeritoFinalDto>> Handle(GetCuadroMeritoFinalQuery request, CancellationToken cancellationToken)
        {
            return await _comiteService.ObtenerCuadroMeritoFinalAsync(request.IdPlaza);
        }
    }
}
