using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Postulantes.Dtos.FichaPostulante
{
    public class ExperienciaReporte
    {
        public string EmpresaInstitucion { get; set; } = string.Empty;
        public string CargoPuesto { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string Sector { get; set; } = string.Empty;
        public string Regimen { get; set; } = string.Empty;
        public decimal Remuneracion { get; set; }
        public string Funciones { get; set; } = string.Empty;
    }
}
