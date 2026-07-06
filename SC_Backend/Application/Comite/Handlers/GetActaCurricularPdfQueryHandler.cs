using Application.Comite.Queries;
using Application.Common.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Comite.Handlers
{
    public class GetActaCurricularPdfQueryHandler : IRequestHandler<GetActaCurricularPdfQuery, byte[]>
    {
        private readonly IMediator _mediator;
        private readonly IComiteEvaluadorService _comiteService;

        public GetActaCurricularPdfQueryHandler(IMediator mediator, IComiteEvaluadorService comiteService)
        {
            _mediator = mediator;
            _comiteService = comiteService;
        }

        public async Task<byte[]> Handle(GetActaCurricularPdfQuery request, CancellationToken cancellationToken)
        {
            // Llamada limpia via MediatR sin repetir código de HTTP client
            var candidatos = await _mediator.Send(new GetCalificacionCurricularQuery(request.IdPlaza), cancellationToken);
            var primerCandidato = candidatos.FirstOrDefault();

            string codigoConvocatoria = primerCandidato?.CodigoConvocatoria ?? "CONVOCATORIA CAS";
            string nombrePuesto = primerCandidato?.NombrePuesto ?? "PLAZA SELECCIONADA";

            return await _comiteService.ObtenerActaCurricularPdfAsync(candidatos, codigoConvocatoria.ToUpper(), nombrePuesto.ToUpper());
        }
    }
}
