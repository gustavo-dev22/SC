using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Comite.Dtos
{
    public class EvaluacionEntrevistaDto
    {
        public int IdPostulacion { get; set; }
        public string CodigoPostulacionUnid { get; set; } = null!;
        public int IdPostulante { get; set; }
        public string PostulanteNombre { get; set; } = null!;
        public int IdEstadoPostulacionCat { get; set; }
        public DateTime FechaPostulacion { get; set; }
        public decimal NotaEntrevista { get; set; }
        public string CodigoConvocatoria { get; set; } = "CONVOCATORIA CAS";
        public string NombrePuesto { get; set; } = "PLAZA SELECCIONADA";
        public bool? FaseEntrevistaAprobado { get; set; }
    }
}
