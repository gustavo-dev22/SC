using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Postulantes.Commands;
using MediatR;

namespace Application.Postulantes.Handlers
{
    public class GuardarDeclaracionesCommandHandler : IRequestHandler<GuardarDeclaracionesCommand, bool>
    {
        private readonly IPostulanteCommandService _commandService;
        public GuardarDeclaracionesCommandHandler(IPostulanteCommandService commandService) => _commandService = commandService;

        public async Task<bool> Handle(GuardarDeclaracionesCommand request, CancellationToken cancellationToken)
        {
            return await _commandService.GuardarDeclaracionesAsync(request.IdPostulante, request.Declaraciones);
        }
    }
}
