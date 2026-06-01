using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Postulantes.Dtos
{
    public class PostulanteFormacionDto
    {
        public int IdFormacion { get; set; }
        public int IdPostulante { get; set; }
        public int IdNivelCat { get; set; }
        public string NivelClasificacion { get; set; } = string.Empty;
        public int IdEstadoCat { get; set; }
        public string EstadoActual { get; set; } = string.Empty;
        public string Institucion { get; set; } = string.Empty;
        public string Carrera { get; set; } = string.Empty;
        public int MesInicio { get; set; }
        public int AnioInicio { get; set; }
        public int? MesFin { get; set; }
        public int? AnioFin { get; set; }
        public string? RutaSustento { get; set; }
    }
}
