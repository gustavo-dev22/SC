using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Dtos;
using MediatR;

namespace Application.Postulantes.Queries
{
    public record GetDepartamentosQuery() : IRequest<List<UbigeoDto>>;

    public record GetProvinciasQuery(string IdDepartamento) : IRequest<List<UbigeoDto>>;

    public record GetDistritosQuery(string IdProvincia) : IRequest<List<UbigeoDto>>;
}
