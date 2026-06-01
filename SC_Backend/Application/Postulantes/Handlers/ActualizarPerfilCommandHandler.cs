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
    public class ActualizarPerfilCommandHandler : IRequestHandler<ActualizarPerfilCommand, bool>
    {
        private readonly IPostulanteCommandService _commandService;
        public ActualizarPerfilCommandHandler(IPostulanteCommandService srv) => _commandService = srv;

        public async Task<bool> Handle(ActualizarPerfilCommand request, CancellationToken token)
        {
            return await _commandService.ActualizarPerfilAsync(request);
        }
    }
}
