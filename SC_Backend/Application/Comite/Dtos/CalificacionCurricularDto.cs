using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Comite.Dtos
{
    public class CalificacionCurricularDto
    {
        public int IdPostulacion { get; set; }
        public string CodigoPostulacionUnid { get; set; } = null!;
        public int IdPostulante { get; set; }
        public string PostulanteNombre { get; set; } = null!;
        public string NombrePuesto { get; set; } = null!;
        public string CodigoConvocatoria { get; set; } = null!;
        public DateTime FechaPostulacion { get; set; }
        public int IdEstadoPostulacionCat { get; set; }
        public decimal NotaFormacion { get; set; }
        public decimal NotaCapacitacion { get; set; }
        public decimal NotaExperiencia { get; set; }
        public decimal NotaCurricularFinal { get; set; }
    }
}
