using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Catalogos.Dtos;
using MediatR;

namespace Application.Catalogos.Queries
{
    public record GetCatalogoTiposQuery() : IRequest<List<CatalogoTipoDto>>;
}
