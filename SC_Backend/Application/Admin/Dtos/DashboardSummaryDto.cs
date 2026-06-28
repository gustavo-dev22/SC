using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Admin.Dtos
{
    public class DashboardSummaryDto
    {
        public DashboardMetricasDto Metricas { get; set; } = new();
        public List<DashboardEstadoDistribucionDto> DistribucionEstados { get; set; } = new();
    }

    public class DashboardMetricasDto
    {
        public int TotalPostulantes { get; set; }
        public int TotalPostulacionesAnio { get; set; }
        public int TicketsSoportePendientes { get; set; }
    }

    public class DashboardEstadoDistribucionDto
    {
        public string EstadoDescripcion { get; set; } = string.Empty;
        public int Total { get; set; }
    }
}
