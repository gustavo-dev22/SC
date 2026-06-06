using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Postulantes.Dtos
{
    public class PostulanteOfimaticaDto
    {
        public int IdPostulanteOfimatica { get; set; }
        public int IdPostulante { get; set; }
        public int IdHerramientaCat { get; set; }
        public string HerramientaClasificacion { get; set; } = string.Empty;
        public int IdNivelCat { get; set; }
        public string NivelClasificacion { get; set; } = string.Empty;
    }
}
