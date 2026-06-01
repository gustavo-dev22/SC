using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Postulantes.Commands
{
    public record RegistrarPostulanteCommand(
        string NumDocumento,
        string Nombres,
        string ApellidoPaterno,
        string ApellidoMaterno,
        string Correo,
        string Password
    ) : IRequest<bool>;
}
