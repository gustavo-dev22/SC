using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Postulantes.Dtos
{
    public class PostulanteDeclaracionDto
    {
        public int IdDeclaracionCat { get; set; }
        public string TituloDeclaracion { get; set; } = string.Empty;
        public string TextoLegal { get; set; } = string.Empty;
        public bool Aceptado { get; set; }
    }

    public class GuardarDeclaracionItemDto
    {
        public int IdDeclaracionCat { get; set; }
        public bool Aceptado { get; set; }
    }
}
