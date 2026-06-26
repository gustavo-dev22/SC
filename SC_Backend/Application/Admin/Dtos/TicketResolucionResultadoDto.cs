using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Admin.Dtos
{
    public class TicketResolucionResultadoDto
    {
        public int IdPostulante { get; set; }
        public string Asunto { get; set; } = string.Empty;
    }
}
