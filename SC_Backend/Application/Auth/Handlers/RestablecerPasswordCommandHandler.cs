using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Auth.Commands;
using Application.Common.Interfaces;
using MediatR;

namespace Application.Auth.Handlers
{
    public class RestablecerPasswordCommandHandler : IRequestHandler<RestablecerPasswordCommand, bool>
    {
        private readonly IPostulanteCommandService _commandService;

        public RestablecerPasswordCommandHandler(IPostulanteCommandService commandService)
        {
            _commandService = commandService;
        }

        public async Task<bool> Handle(RestablecerPasswordCommand request, CancellationToken cancellationToken)
        {
            // 🚀 AQUÍ: Inyectas tu encriptación actual (ej. BCrypt o la lógica hash que uses en el registro)
            string passwordEncriptado = BCrypt.Net.BCrypt.HashPassword(request.NuevoPassword);

            return await _commandService.RestablecerPasswordAsync(request.Token, passwordEncriptado);
        }
    }
}
