using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Comite.Queries;
using Application.Common.Interfaces;
using MediatR;

namespace Application.Comite.Handlers
{
    public class GetActaEntrevistaPdfQueryHandler : IRequestHandler<GetActaEntrevistaPdfQuery, byte[]>
    {
        private readonly IMediator _mediator;
        private readonly IComiteEvaluadorService _comiteService;

        public GetActaEntrevistaPdfQueryHandler(IMediator mediator, IComiteEvaluadorService comiteService)
        {
            _mediator = mediator;
            _comiteService = comiteService;
        }

        public async Task<byte[]> Handle(GetActaEntrevistaPdfQuery request, CancellationToken cancellationToken)
        {
            // Reutilizamos de forma interna el Handler que jala las notas y los strings reales de Java Spring Boot 🚀
            var candidatos = await _mediator.Send(new GetEvaluacionEntrevistaQuery(request.IdPlaza), cancellationToken);
            var primerCandidato = candidatos.FirstOrDefault();

            string codigoConvocatoria = primerCandidato?.CodigoConvocatoria ?? "CONVOCATORIA CAS";
            string nombrePuesto = primerCandidato?.NombrePuesto ?? "PLAZA SELECCIONADA";

            return await _comiteService.ObtenerActaEntrevistaPdfAsync(candidatos, codigoConvocatoria.ToUpper(), nombrePuesto.ToUpper());
        }
    }
}
