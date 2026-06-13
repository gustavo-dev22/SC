using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Auth.Dtos
{
    public class SolicitudRecuperacionResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? LinkDesarrollo { get; set; } // 🚀 Clave para saltearnos el SMTP en local
    }

    public class DatosPostulanteTokenDto
    {
        public int IdPostulante { get; set; }
        public string Nombres { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
    }
}
