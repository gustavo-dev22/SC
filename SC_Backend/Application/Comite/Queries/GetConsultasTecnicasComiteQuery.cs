using Application.Admin.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Comite.Queries
{
    public record GetConsultasTecnicasComiteQuery(int? IdEstado, string? Busqueda) : IRequest<List<AdminTicketBandejaDto>>;
}
