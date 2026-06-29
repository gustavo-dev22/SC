using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Application.Comite.Commands
{
    public record EvaluarExpedienteCommand(int IdPostulacion, bool Aprobado, string Observacion) : IRequest<bool>;
}
