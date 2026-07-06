using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Application.Postulantes.Commands
{
    public record EliminarSustentoCommand(int IdRegistro, string Seccion) : IRequest<bool>;
}
