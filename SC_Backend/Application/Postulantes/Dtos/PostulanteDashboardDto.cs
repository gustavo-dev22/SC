using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Postulantes.Dtos
{
    public class PostulanteDashboardDto
    {
        public int IdPostulacion { get; set; }
        public int IdPlaza { get; set; }
        public string CodigoConvocatoria { get; set; } = "N/A"; // Se jalará de Java Spring Boot
        public string NombrePuesto { get; set; } = string.Empty;  // Se jalará de Java Spring Boot
        public string CodigoPostulacionUnid { get; set; } = string.Empty;
        public DateTime FechaPostulacion { get; set; }
        public int IdEstadoPostulacionCat { get; set; }
        public string EstadoPostulacionDescripcion { get; set; } = string.Empty;
    }
}
