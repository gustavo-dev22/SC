using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Postulantes.Commands
{
    public record RegistrarTicketCommand(
        int IdPostulante,
        int? IdPlaza,
        int IdTipoTicketCat,
        string Asunto,
        string Descripcion
    ) : IRequest<bool>;
}
