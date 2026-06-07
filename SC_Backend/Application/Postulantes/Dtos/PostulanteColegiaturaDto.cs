using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Postulantes.Dtos
{
    public class PostulanteColegiaturaDto
    {
        public int IdColegiatura { get; set; }
        public int IdPostulante { get; set; }
        public int IdColegioCat { get; set; }
        public string ColegioProfesionalClasificacion { get; set; } = string.Empty;
        public string NumeroColegiacion { get; set; } = string.Empty;
        public DateTime FechaColegiacion { get; set; }
        public string? CertificadoHabilitacionRuta { get; set; }
        public bool Habilitado { get; set; }
        public string MotivoNoHabilitado { get; set; } = string.Empty;
    }
}
