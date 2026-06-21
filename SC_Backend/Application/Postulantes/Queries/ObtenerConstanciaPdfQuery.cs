using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Application.Postulantes.Queries
{
    public record ObtenerConstanciaPdfQuery(int IdPostulacion) : IRequest<byte[]>;
}
