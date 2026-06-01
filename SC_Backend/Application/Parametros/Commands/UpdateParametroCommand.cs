using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Application.Parametros.Commands
{
    public record MantenimientoParametroCommand(
        string Accion,
        string Codigo,
        string Nombre,
        string Valor,
        string Descripcion,
        string Categoria
    ) : IRequest<bool>;
}
