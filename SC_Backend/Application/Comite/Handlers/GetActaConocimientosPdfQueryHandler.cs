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
    public class GetActaConocimientosPdfQueryHandler : IRequestHandler<GetActaConocimientosPdfQuery, byte[]>
    {
        private readonly IMediator _mediator;
        private readonly IComiteEvaluadorService _comiteQueryService;

        public GetActaConocimientosPdfQueryHandler(IMediator mediator, IComiteEvaluadorService comiteQueryService)
        {
            _mediator = mediator;
            _comiteQueryService = comiteQueryService;
        }

        public async Task<byte[]> Handle(GetActaConocimientosPdfQuery request, CancellationToken cancellationToken)
        {
            // Reutilizamos el Handler que lista los candidatos de conocimientos
            var candidatos = await _mediator.Send(new GetEvaluacionConocimientosQuery(request.IdPlaza), cancellationToken);

            // 🚀 SOLUCIÓN: Buscamos el primer registro y extraemos sus metadatos reales cruzados
            var primerCandidato = candidatos.FirstOrDefault();

            // Si la lista está vacía, usamos un fail-safe genérico, de lo contrario extraemos el valor real
            // Nota: Asegúrate de que las propiedades del DTO 'EvaluacionConocimientosDto' incluyan estos campos o los hereden
            string codigoConvocatoria = primerCandidato?.CodigoConvocatoria ?? "CONVOCATORIA CAS";
            string nombrePuesto = primerCandidato?.NombrePuesto ?? "PLAZA SELECCIONADA";

            return await _comiteQueryService.ObtenerActaConocimientosPdfAsync(candidatos, codigoConvocatoria.ToUpper(), nombrePuesto.ToUpper());
        }
    }
}
