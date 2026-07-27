using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Comite.Dtos
{
    public class DeclararPlazaDesiertaDto
    {
        public int IdPlaza { get; set; }
        public int IdMotivoDesiertaCat { get; set; }
        public string SustentoDesierta { get; set; } = string.Empty;
    }
}
