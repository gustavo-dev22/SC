using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Postulantes.Dtos
{
    public class PostulanteIdiomaDto
    {
        public int IdPostulanteIdioma { get; set; }
        public int IdPostulante { get; set; }
        public int IdIdiomaCat { get; set; }
        public string IdiomaClasificacion { get; set; } = string.Empty;
        public int IdNivelHablaCat { get; set; }
        public string NivelHablaClasificacion { get; set; } = string.Empty;
        public int IdNivelLecturaCat { get; set; }
        public string NivelLecturaClasificacion { get; set; } = string.Empty;
        public int IdNivelEscrituraCat { get; set; }
        public string NivelEscrituraClasificacion { get; set; } = string.Empty;
    }
}
