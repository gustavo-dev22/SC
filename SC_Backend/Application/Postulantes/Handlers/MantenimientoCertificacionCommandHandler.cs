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
    public class MantenimientoCertificacionCommandHandler : IRequestHandler<MantenimientoCertificacionCommand, bool>
    {
        private readonly IPostulanteCommandService _commandService;

        public MantenimientoCertificacionCommandHandler(IPostulanteCommandService commandService)
        {
            _commandService = commandService;
        }

        public async Task<bool> Handle(MantenimientoCertificacionCommand request, CancellationToken cancellationToken)
        {
            return await _commandService.MantenimientoCertificacionAsync(request);
        }
    }
}
