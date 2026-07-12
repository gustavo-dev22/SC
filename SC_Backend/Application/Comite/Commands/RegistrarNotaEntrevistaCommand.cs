using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Application.Comite.Commands
{
    public record RegistrarNotaEntrevistaCommand(int IdPostulacion, decimal NotaEntrevista) : IRequest<bool>;
}
