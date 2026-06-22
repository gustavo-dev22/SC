using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Postulantes.Dtos
{
    public class NotificacionDto
    {
        public int IdNotificacion { get; set; }
        public int IdPostulante { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
        public int IdTipoAlertaCat { get; set; } // 1=Info, 2=Urgente, 3=Exito
        public bool Leido { get; set; }
        public DateTime FechaEnvio { get; set; }
    }
}
