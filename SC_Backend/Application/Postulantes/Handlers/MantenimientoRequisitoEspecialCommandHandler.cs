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
    public class MantenimientoRequisitoEspecialCommandHandler : IRequestHandler<MantenimientoRequisitoEspecialCommand, bool>
    {
        private readonly IPostulanteCommandService _commandService;
        public MantenimientoRequisitoEspecialCommandHandler(IPostulanteCommandService commandService) => _commandService = commandService;

        public async Task<bool> Handle(MantenimientoRequisitoEspecialCommand request, CancellationToken cancellationToken)
        {
            return await _commandService.MantenimientoRequisitoEspecialAsync(request);
        }
    }
}
