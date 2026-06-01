using Application.Catalogos.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Catalogos.Queries
{
    public record GetValoresByCodigoTipoQuery(string Codigo) : IRequest<List<CatalogoValorDto>>;
}
