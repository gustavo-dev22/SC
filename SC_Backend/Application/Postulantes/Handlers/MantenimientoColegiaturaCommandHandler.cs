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
    public class MantenimientoColegiaturaCommandHandler : IRequestHandler<MantenimientoColegiaturaCommand, bool>
    {
        private readonly IPostulanteCommandService _commandService;
        public MantenimientoColegiaturaCommandHandler(IPostulanteCommandService commandService) => _commandService = commandService;

        public async Task<bool> Handle(MantenimientoColegiaturaCommand request, CancellationToken cancellationToken)
        {
            return await _commandService.MantenimientoColegiaturaAsync(request);
        }
    }
}
