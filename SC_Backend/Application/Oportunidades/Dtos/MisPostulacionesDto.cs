using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Oportunidades.Dtos
{
    public class MisPostulacionesDto
    {
        public int IdPostulacion { get; set; }
        public int IdPlaza { get; set; }
        public int IdPostulante { get; set; }
        public string CodigoPostulacion { get; set; } = string.Empty;
        public DateTime FechaPostulacion { get; set; }

        // Datos del catálogo local
        public int IdEstadoPostulacionCat { get; set; }
        public string EstadoCodigo { get; set; } = string.Empty; // INS, APT_CV, etc.
        public string EstadoDescripcion { get; set; } = string.Empty; // Inscrito, No Apto...

        // Datos que traeremos complementariamente de las plazas (Java/Simulado)
        public string CodigoConvocatoria { get; set; } = string.Empty;
        public string NombrePuesto { get; set; } = string.Empty;
        public string UnidadOrganica { get; set; } = string.Empty;
        public decimal Remuneracion { get; set; }
    }
}
