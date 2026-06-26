using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Admin.Dtos
{
    public class TrazabilidadExpedienteDto
    {
        public int IdPostulacionHistorial { get; set; }
        public int IdPostulante { get; set; }
        public string PostulanteNombreCompleto { get; set; } = string.Empty;
        public string PostulanteDni { get; set; } = string.Empty;
        public int IdPlaza { get; set; }
        public string CodigoConvocatoria { get; set; } = "N/A"; // Se jalará de Java
        public string NombrePuesto { get; set; } = string.Empty;  // Se jalará de Java
        public string CodigoExpediente { get; set; } = string.Empty;
        public int IdEstadoPostulacionCat { get; set; }
        public string EstadoPostulacionDescripcion { get; set; } = string.Empty;
        public string? Observacion { get; set; }
        public DateTime FechaCambio { get; set; }
        public string UsuarioOperacion { get; set; } = string.Empty;
    }
}
