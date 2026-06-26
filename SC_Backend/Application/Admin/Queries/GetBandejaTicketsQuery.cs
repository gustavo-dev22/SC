using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Admin.Dtos;
using MediatR;

namespace Application.Admin.Queries
{
    public record GetBandejaTicketsQuery(int? IdEstado, string? Busqueda) : IRequest<List<AdminTicketBandejaDto>>;
}
