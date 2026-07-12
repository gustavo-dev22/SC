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
    public class RegistrarNotaEntrevistaCommandHandler : IRequestHandler<RegistrarNotaEntrevistaCommand, bool>
    {
        private readonly IComiteEvaluadorService _comiteService;
        public RegistrarNotaEntrevistaCommandHandler(IComiteEvaluadorService comiteService) => _comiteService = comiteService;

        public async Task<bool> Handle(RegistrarNotaEntrevistaCommand request, CancellationToken cancellationToken)
        {
            return await _comiteService.RegistrarNotaEntrevistaAsync(request.IdPostulacion, request.NotaEntrevista);
        }
    }
}
