using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Application.Comite.Queries;
using Application.Common.Interfaces;
using MediatR;

namespace Application.Comite.Handlers
{
    public class GetActaInicialPdfQueryHandler : IRequestHandler<GetActaInicialPdfQuery, byte[]>
    {
        private readonly IMediator _mediator;
        private readonly IComiteEvaluadorService _comiteQueryService;

        public GetActaInicialPdfQueryHandler(IMediator mediator, IComiteEvaluadorService comiteQueryService)
        {
            _mediator = mediator;
            _comiteQueryService = comiteQueryService;
        }

        public async Task<byte[]> Handle(GetActaInicialPdfQuery request, CancellationToken cancellationToken)
        {
            // 1. Reutilizamos tu Query de listado que ya sabe ir a SQL Server y rellenar los datos cruzados de Java
            var expedientes = await _mediator.Send(new GetExpedientesInscritosQuery(request.IdPlaza), cancellationToken);

            // 2. Extraemos los nombres reales recuperados
            string codigoConvocatoria = (expedientes.FirstOrDefault()?.CodigoConvocatoria ?? "CONVOCATORIA CAS").ToUpper();
            string nombrePuesto = (expedientes.FirstOrDefault()?.NombrePuesto ?? "PLAZA SELECCIONADA").ToUpper();

            // 3. Enviamos los objetos limpios a Infraestructura para que arme el PDF en memoria pura
            return await _comiteQueryService.ObtenerActaInicialPdfAsync(expedientes, codigoConvocatoria, nombrePuesto);
        }
    }
}
