using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Comite.Commands;
using Application.Common.Interfaces;
using MediatR;

namespace Application.Comite.Handlers
{
    public class RegistrarNotaConocimientosCommandHandler : IRequestHandler<RegistrarNotaConocimientosCommand, bool>
    {
        private readonly IComiteEvaluadorService _comiteService;

        public RegistrarNotaConocimientosCommandHandler(IComiteEvaluadorService comiteService)
        {
            _comiteService = comiteService;
        }

        public async Task<bool> Handle(RegistrarNotaConocimientosCommand request, CancellationToken cancellationToken)
        {
            return await _comiteService.RegistrarNotaConocimientosAsync(request.IdPostulacion, request.NotaConocimientos);
        }
    }
}
