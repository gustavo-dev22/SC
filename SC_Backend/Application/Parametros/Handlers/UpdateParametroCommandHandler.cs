using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Parametros.Commands;
using MediatR;

namespace Application.Parametros.Handlers
{
    public class MantenimientoParametroCommandHandler : IRequestHandler<MantenimientoParametroCommand, bool>
    {
        private readonly IParametroCommandService _commandService;

        public MantenimientoParametroCommandHandler(IParametroCommandService srv)
        {
            _commandService = srv;
        }

        public async Task<bool> Handle(MantenimientoParametroCommand request, CancellationToken token)
        {
            return await _commandService.ProcesarMantenimientoAsync(request);
        }
    }
}
