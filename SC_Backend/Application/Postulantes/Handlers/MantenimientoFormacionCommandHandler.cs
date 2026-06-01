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
    public class MantenimientoFormacionCommandHandler : IRequestHandler<MantenimientoFormacionCommand, bool>
    {
        private readonly IPostulanteCommandService _commandService;

        public MantenimientoFormacionCommandHandler(IPostulanteCommandService commandService)
        {
            _commandService = commandService;
        }

        public async Task<bool> Handle(MantenimientoFormacionCommand request, CancellationToken cancellationToken)
        {
            return await _commandService.MantenimientoFormacionAsync(request);
        }
    }
}
