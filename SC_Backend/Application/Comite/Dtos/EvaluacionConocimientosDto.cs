using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Comite.Dtos
{
    public class EvaluacionConocimientosDto
    {
        public int IdPostulacion { get; set; }
        public string CodigoConvocatoria { get; set; } = string.Empty;
        public string NombrePuesto { get; set; } = string.Empty;
        public string CodigoPostulacionUnid { get; set; } = string.Empty;
        public string PostulanteNombre { get; set; } = string.Empty;
        public int IdEstadoPostulacionCat { get; set; }
        public string EstadoDescripcion { get; set; } = string.Empty;
        public decimal NotaConocimientos { get; set; }
        public bool? FaseConocimientosAprobado { get; set; }
    }
}
