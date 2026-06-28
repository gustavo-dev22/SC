using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Comite.Dtos
{
    public class ExpedienteInscritoDto
    {
        public int IdPostulacion { get; set; }
        public int IdPostulante { get; set; }
        public int IdPlaza { get; set; }
        public string CodigoConvocatoria { get; set; } = "N/A"; // De Java
        public string NombrePuesto { get; set; } = string.Empty;  // De Java
        public string CodigoPostulacionUnid { get; set; } = string.Empty;
        public DateTime FechaPostulacion { get; set; }
        public int IdEstadoPostulacionCat { get; set; }
        public string EstadoDescripcion { get; set; } = string.Empty;
        // Datos del postulante que inyectaremos (puedes unirlos de tu microservicio de personas si aplica)
        public string PostulanteNombre { get; set; } = "Postulante Evaluado";
    }
}
