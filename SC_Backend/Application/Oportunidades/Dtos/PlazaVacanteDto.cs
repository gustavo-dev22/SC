using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Oportunidades.Dtos
{
    public class PlazaVacanteDto
    {
        public int IdPlaza { get; set; }
        public string CodigoConvocatoria { get; set; } = string.Empty;
        public string NombrePuesto { get; set; } = string.Empty;
        public string UnidadOrganica { get; set; } = string.Empty;
        public decimal Remuneracion { get; set; }
        public DateTime FechaFin { get; set; }
        public bool YaPostulo { get; set; } // 🚀 Propiedad calculada localmente con Dapper
    }
}
