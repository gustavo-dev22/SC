using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Admin.Dtos;
using Application.Admin.Queries;
using Application.Common.Interfaces;
using MediatR;

namespace Application.Admin.Handlers
{
    public class GetLogsAuditoriaQueryHandler : IRequestHandler<GetLogsAuditoriaQuery, List<AuditoriaLogDto>>
    {
        private readonly IAdminSoporteService _adminService;

        public GetLogsAuditoriaQueryHandler(IAdminSoporteService adminService)
        {
            _adminService = adminService;
        }

        public async Task<List<AuditoriaLogDto>> Handle(GetLogsAuditoriaQuery request, CancellationToken cancellationToken)
        {
            // 🚀 CORREGIDO: Transmitimos los parámetros de filtrado avanzados hacia la infraestructura
            return await _adminService.ObtenerLogsAuditoriaAsync(
                request.Tabla,
                request.Operacion,
                request.FechaInicio,
                request.FechaFin
            );
        }
    }
}
