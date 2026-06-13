using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Postulantes.Dtos.FichaPostulante
{
    public class FormacionReporte
    {
        public string NivelEstudio { get; set; } = string.Empty;
        public string EstadoEstudio { get; set; } = string.Empty;
        public string CentroEstudios { get; set; } = string.Empty;
        public string CarreraEspecialidad { get; set; } = string.Empty;
        public string PeriodoFin { get; set; } = string.Empty;
    }
}
