using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Application.Catalogos.Commands
{
    public record TipoCommand(
        string Accion,
        int IdTipo,
        string Codigo,
        string Nombre,
        bool Activo
    ) : IRequest<bool>;
}
