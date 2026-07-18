using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Comite.Dtos
{
    public class ComiteDashboardDto
    {
        public ComiteMetricasDto Metricas { get; set; } = new();
        public List<DistribucionEstadosDto> DistribucionEstados { get; set; } = new();
    }

    public class ComiteMetricasDto
    {
        public int TotalPlazasAsignadas { get; set; }
        public int TotalCandidatosEvaluacion { get; set; }
        public int TicketsSoportePendientes { get; set; }
    }

    public class DistribucionEstadosDto
    {
        public string EstadoDescripcion { get; set; } = string.Empty;
        public int Total { get; set; }
    }
}
