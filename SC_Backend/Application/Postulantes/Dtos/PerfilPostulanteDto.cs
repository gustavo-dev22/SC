using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Postulantes.Dtos
{
    public class PerfilPostulanteDto
    {
        public int IdPostulante { get; set; }
        public string NumDocumento { get; set; } = string.Empty;
        public string Nombres { get; set; } = string.Empty;
        public string ApellidoPaterno { get; set; } = string.Empty;
        public string ApellidoMaterno { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public DateTime? FechaNacimiento { get; set; }
        public int IdSexoCat { get; set; }
        public string Direccion { get; set; } = string.Empty;
        public int IdTipoViaCat { get; set; }
        public string NumeroVia { get; set; }
        public string NumeroDepto { get; set; }
        public string Interior { get; set; }
        public string Manzana { get; set; }
        public string Lote { get; set; }
        public string Kilometro { get; set; }
        public string BlockEdificio { get; set; } = string.Empty;
        public string Etapa { get; set; } = string.Empty;
        public int IdTipoZonaCat { get; set; }
        public string NombreZona { get; set; } = string.Empty;
        public string ReferenciaDireccion { get; set; } = string.Empty;
        public string IdUbigeoDistrito { get; set; } = string.Empty;
        public string IdProvincia { get; set; } = string.Empty;
        public string IdDepartamento { get; set; } = string.Empty;
    }
}
