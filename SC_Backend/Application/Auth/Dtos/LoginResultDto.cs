using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Auth.Dto
{
    public class LoginResultDto
    {
        public bool Success { get; set; }
        public string Token { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public List<MenuObjetoDto> Menus { get; set; } = new();
    }
}
