using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Postulantes.Dtos
{
    public class PostulanteCertificacionDto
    {
        public int IdCertificacion { get; set; }
        public int IdPostulante { get; set; }
        public int IdTipoEstudioCat { get; set; }
        public string TipoEstudioClasificacion { get; set; } = string.Empty;
        public string NombreEstudio { get; set; } = string.Empty;
        public string Institucion { get; set; } = string.Empty;
        public int HorasAcademicas { get; set; }
        public DateTime FechaEmision { get; set; }
        public string? RutaSustento { get; set; }
    }
}
