using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Postulantes.Dtos.FichaPostulante
{
    public class ColegiaturaReporte
    {
        public string ColegioProfesional { get; set; } = string.Empty;
        public string NumeroColegiacion { get; set; } = string.Empty;
        public DateTime? FechaColegiacion { get; set; }
        public string CondicionHabilitado { get; set; } = string.Empty;
    }
}
