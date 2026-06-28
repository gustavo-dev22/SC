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
    public class GetDashboardSummaryQueryHandler : IRequestHandler<GetDashboardSummaryQuery, DashboardSummaryDto>
    {
        private readonly IAdminSoporteService _adminService;

        public GetDashboardSummaryQueryHandler(IAdminSoporteService adminService)
        {
            _adminService = adminService;
        }

        public async Task<DashboardSummaryDto> Handle(GetDashboardSummaryQuery request, CancellationToken cancellationToken)
        {
            return await _adminService.ObtenerResumenDashboardAsync();
        }
    }
}
