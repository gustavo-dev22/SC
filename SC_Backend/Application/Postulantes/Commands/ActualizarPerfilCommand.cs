using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Postulantes.Commands
{
    public record ActualizarPerfilCommand(
        int IdPostulante,
        string Telefono,
        DateTime FechaNacimiento,
        int IdSexoCat,
        string Direccion
    ) : IRequest<bool>;
}
