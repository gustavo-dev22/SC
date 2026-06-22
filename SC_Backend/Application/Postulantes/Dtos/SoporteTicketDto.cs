using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Postulantes.Dtos
{
    public class SoporteTicketDto
    {
        public int IdTicket { get; set; }
        public int? IdPlaza { get; set; }
        public int IdTipoTicketCat { get; set; }
        public int IdEstadoTicketCat { get; set; }
        public string Asunto { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string? RespuestaSoporte { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime? FechaRespuesta { get; set; }

        // Propiedades extendidas para visualización
        public string CodigoConvocatoria { get; set; } = "N/A";
        public string TipoTicketDescripcion { get; set; } = string.Empty;
        public string EstadoTicketDescripcion { get; set; } = string.Empty;
    }
}
