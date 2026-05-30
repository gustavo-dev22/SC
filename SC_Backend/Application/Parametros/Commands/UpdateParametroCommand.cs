using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Application.Parametros.Commands
{
    public record UpdateParametroCommand(string Codigo, string Valor) : IRequest<bool>;
}
