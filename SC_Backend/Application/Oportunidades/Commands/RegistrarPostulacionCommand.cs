using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Application.Oportunidades.Commands
{
    public record RegistrarPostulacionCommand(
        int IdPostulante,
        int IdPlaza,
        DateTime FechaFinPlaza,
        bool YaPostulo
    ) : IRequest<bool>;
}
