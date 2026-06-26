using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Admin.Dtos
{
    public class AdminTicketBandejaDto
    {
        public int IdTicket { get; set; }
        public int IdPostulante { get; set; }
        public string PostulanteNombreCompleto { get; set; } = string.Empty;
        public string PostulanteDni { get; set; } = string.Empty;
        public int? IdPlaza { get; set; }
        public string CodigoConvocatoria { get; set; } = "N/A";
        public int IdTipoTicketCat { get; set; }
        public string TipoTicketDescripcion { get; set; } = string.Empty;
        public int IdEstadoTicketCat { get; set; }
        public string EstadoTicketDescripcion { get; set; } = string.Empty;
        public string Asunto { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string? RespuestaSoporte { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime? FechaRespuesta { get; set; }
    }
}
