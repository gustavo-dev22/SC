using Application.Admin.Dtos;
using Application.Comite.Queries;
using Application.Common.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Comite.Handlers
{
    public class GetConsultasTecnicasComiteQueryHandler : IRequestHandler<GetConsultasTecnicasComiteQuery, List<AdminTicketBandejaDto>>
    {
        private readonly IComiteEvaluadorService _comiteService;
        public GetConsultasTecnicasComiteQueryHandler(IComiteEvaluadorService comiteService) => _comiteService = comiteService;

        public async Task<List<AdminTicketBandejaDto>> Handle(GetConsultasTecnicasComiteQuery request, CancellationToken cancellationToken)
        {
            return await _comiteService.ObtenerConsultasTecnicasAsync(request.IdEstado, request.Busqueda);
        }
    }
}
