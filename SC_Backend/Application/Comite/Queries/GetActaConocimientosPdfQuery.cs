using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Application.Comite.Queries
{
    public record GetActaConocimientosPdfQuery(int IdPlaza) : IRequest<byte[]>;
}
