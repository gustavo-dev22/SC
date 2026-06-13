using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Postulantes.Dtos.FichaPostulante
{
    public class CertificacionReporte
    {
        public string Tipo { get; set; } = string.Empty;
        public string NombreCurso { get; set; } = string.Empty;
        public string InstitucionEmisora { get; set; } = string.Empty;
        public int HorasLectivas { get; set; }
        public DateTime? FechaCertificacion { get; set; }
    }
}
