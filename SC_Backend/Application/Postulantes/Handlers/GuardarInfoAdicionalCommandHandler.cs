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
    public class GuardarInfoAdicionalCommandHandler : IRequestHandler<GuardarInfoAdicionalCommand, bool>
    {
        private readonly IPostulanteCommandService _commandService;

        public GuardarInfoAdicionalCommandHandler(IPostulanteCommandService commandService)
        {
            _commandService = commandService;
        }

        public async Task<bool> Handle(GuardarInfoAdicionalCommand request, CancellationToken cancellationToken)
        {
            return await _commandService.GuardarInfoAdicionalAsync(request);
        }
    }
}
