using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Models.Integration.Sasi
{
    public class SasiIntegrationModels
    {
        public class SasiResponseModel
        {
            public bool Success { get; set; }
            public string Token { get; set; } = string.Empty;
            public SasiUsuarioModel Usuario { get; set; } = new();
        }
        public class SasiUsuarioModel
        {
            public string NombreCompleto { get; set; } = string.Empty;
            public List<SasiSistemaModel> Sistemas { get; set; } = new();
        }
        public class SasiSistemaModel
        {
            public int Id { get; set; }
            public bool Activo { get; set; }
            public List<SasiRolModel> Roles { get; set; } = new();
        }
        public class SasiRolModel
        {
            public string NombreRol { get; set; } = string.Empty;
            public bool Activo { get; set; }
            public List<SasiObjetoModel> Objetos { get; set; } = new();
        }
        public class SasiObjetoModel
        {
            public int IdObjeto { get; set; }
            public int? IdPadre { get; set; }
            public string Nombre { get; set; } = string.Empty;
            public string Tipo { get; set; } = string.Empty;
            public string Url { get; set; } = string.Empty;
            public string Icono { get; set; } = string.Empty;
        }
    }
}
