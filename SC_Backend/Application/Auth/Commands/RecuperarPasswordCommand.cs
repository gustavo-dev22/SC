using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Auth.Dtos;
using MediatR;

namespace Application.Auth.Commands
{
    // 1. Comando para pedir el inicio del flujo enviando el DNI
    public record SolicitarRecuperacionCommand(string NumDocumento) : IRequest<SolicitudRecuperacionResultDto>;

    // 2. Comando para insertar la nueva clave usando el token recibido
    public record RestablecerPasswordCommand(string Token, string NuevoPassword) : IRequest<bool>;
}
