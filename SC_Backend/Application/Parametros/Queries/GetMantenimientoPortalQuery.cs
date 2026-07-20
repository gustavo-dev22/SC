using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Parametros.Queries
{
    public record GetMantenimientoPortalQuery : IRequest<bool>;
}
