using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Postulantes.Dtos.FichaPostulante
{
    public class CabeceraPostulanteReporte
    {
        public int IdPostulante { get; set; }
        public string NumDocumento { get; set; } = string.Empty;
        public string Nombres { get; set; } = string.Empty;
        public string ApellidoPaterno { get; set; } = string.Empty;
        public string ApellidoMaterno { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public DateTime? FechaNacimiento { get; set; }
        public string Direccion { get; set; } = string.Empty;
        public string ReferenciaDireccion { get; set; } = string.Empty;
        public string Sexo { get; set; } = string.Empty;
        public string UbigeoCompleto { get; set; } = string.Empty;
    }
}
