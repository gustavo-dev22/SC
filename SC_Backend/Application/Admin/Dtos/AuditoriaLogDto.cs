using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Admin.Dtos
{
    public class AuditoriaLogDto
    {
        public int IdAuditoria { get; set; }
        public string TablaAfectada { get; set; } = string.Empty;
        public string Operacion { get; set; } = string.Empty;
        public string? ValoresAnteriores { get; set; }
        public string? ValoresNuevos { get; set; }
        public DateTime FechaEvento { get; set; }
        public string UsuarioDb { get; set; } = string.Empty;
        public string AppOrigen { get; set; } = string.Empty;
    }
}
