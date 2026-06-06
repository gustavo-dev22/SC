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
    public class MantenimientoOfimaticaCommandHandler : IRequestHandler<MantenimientoOfimaticaCommand, bool>
    {
        private readonly IPostulanteCommandService _commandService;
        public MantenimientoOfimaticaCommandHandler(IPostulanteCommandService commandService) => _commandService = commandService;

        public async Task<bool> Handle(MantenimientoOfimaticaCommand request, CancellationToken cancellationToken)
        {
            return await _commandService.MantenimientoOfimaticaAsync(request);
        }
    }
}
