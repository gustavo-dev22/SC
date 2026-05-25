using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Catalogos.Commands
{
    public record CatalogoCommand(
        string Accion,
        int IdValor,
        int IdTipo,
        string CodigoValor,
        string Descripcion,
        int Orden,
        bool Activo
    ) : IRequest<bool>;
}
