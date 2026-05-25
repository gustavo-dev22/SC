using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Catalogos.Dtos
{
    public class CatalogoValorDto
    {
        public int IdValor { get; set; }
        public int IdTipo { get; set; }
        public string CodigoValor { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public int Orden { get; set; }
        public bool Activo { get; set; }
    }
}
