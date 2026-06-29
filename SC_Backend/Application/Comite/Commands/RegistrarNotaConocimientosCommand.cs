using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using MediatR;

namespace Application.Comite.Commands
{
    public class RegistrarNotaConocimientosCommand : IRequest<bool>
    {
        [JsonPropertyName("idPostulacion")]
        public int IdPostulacion { get; set; }

        [JsonPropertyName("notaConocimientos")]
        public decimal NotaConocimientos { get; set; }
    }
}
