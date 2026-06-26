using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Application.Admin.Commands
{
    public record AtenderTicketCommand(int IdTicket, string? RespuestaSoporte, int IdEstado, string NombreAdmin) : IRequest<bool>;
}
