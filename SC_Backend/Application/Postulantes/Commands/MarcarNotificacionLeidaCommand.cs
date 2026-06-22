using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Postulantes.Commands
{
    public record MarcarNotificacionLeidaCommand(int IdNotificacion) : IRequest<bool>;
}
