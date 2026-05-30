using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Parametros.Dtos;
using MediatR;

namespace Application.Parametros.Queries
{
    public record GetParametrosQuery() : IRequest<List<ParametroGlobalDto>>;
}
