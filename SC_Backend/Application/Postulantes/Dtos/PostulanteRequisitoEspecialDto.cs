using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Postulantes.Dtos
{
    public class PostulanteRequisitoEspecialDto
    {
        public int IdRequisitoEspecial { get; set; }
        public int IdPostulante { get; set; }
        public int IdTipoRequisitoCat { get; set; }
        public string TipoRequisitoClasificacion { get; set; } = string.Empty;
        public string DescripcionDocumento { get; set; } = string.Empty;
        public string NumeroRegistro { get; set; } = string.Empty;
        public DateTime? FechaEmision { get; set; }
        public DateTime? FechaVencimiento { get; set; }
    }
}
