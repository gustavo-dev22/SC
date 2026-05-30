using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Catalogos.Commands;
using Application.Common.Interfaces;
using MediatR;

namespace Application.Catalogos.Handlers
{
    public class TipoCommandHandler : IRequestHandler<TipoCommand, bool>
    {
        private readonly ICatalogoCommandService _commandService;

        public TipoCommandHandler(ICatalogoCommandService commandService)
        {
            _commandService = commandService;
        }

        public async Task<bool> Handle(TipoCommand request, CancellationToken cancellationToken)
        {
            return await _commandService.ProcesarMantenimientoTipoAsync(request);
        }
    }
}
