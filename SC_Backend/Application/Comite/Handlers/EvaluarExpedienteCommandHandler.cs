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
    public class EvaluarExpedienteCommandHandler : IRequestHandler<EvaluarExpedienteCommand, bool>
    {
        private readonly IComiteEvaluadorService _comiteService;

        public EvaluarExpedienteCommandHandler(IComiteEvaluadorService comiteService)
        {
            _comiteService = comiteService;
        }

        public async Task<bool> Handle(EvaluarExpedienteCommand request, CancellationToken cancellationToken)
        {
            return await _comiteService.EvaluarExpedienteInicialAsync(
                request.IdPostulacion,
                request.Aprobado,
                request.Observacion
            );
        }
    }
}
