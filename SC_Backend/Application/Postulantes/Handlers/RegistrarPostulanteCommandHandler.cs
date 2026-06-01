using Application.Common.Interfaces;
using Application.Postulantes.Commands;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Postulantes.Handlers
{
    public class RegistrarPostulanteCommandHandler : IRequestHandler<RegistrarPostulanteCommand, bool>
    {
        private readonly IPostulanteCommandService _commandService;
        public RegistrarPostulanteCommandHandler(IPostulanteCommandService srv) => _commandService = srv;

        public async Task<bool> Handle(RegistrarPostulanteCommand request, CancellationToken cancellationToken)
        {
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            return await _commandService.RegistrarPostulanteAsync(request, passwordHash);
        }
    }
}
