using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Application.Comite.Queries;
using Application.Common.Interfaces;
using Application.Oportunidades.Dtos;
using MediatR;

namespace Application.Comite.Handlers
{
    public class GetActaFinalPdfQueryHandler : IRequestHandler<GetActaFinalPdfQuery, byte[]>
    {
        private readonly IMediator _mediator;
        private readonly IComiteEvaluadorService _comiteService;
        private readonly IHttpClientFactory _httpClientFactory;

        public GetActaFinalPdfQueryHandler(IMediator mediator, IComiteEvaluadorService comiteService, IHttpClientFactory httpClientFactory)
        {
            _mediator = mediator;
            _comiteService = comiteService;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<byte[]> Handle(GetActaFinalPdfQuery request, CancellationToken cancellationToken)
        {
            var candidatos = await _mediator.Send(new GetCuadroMeritoFinalQuery(request.IdPlaza), cancellationToken);
            var primerCandidato = candidatos.FirstOrDefault();

            string codigoConvocatoria = "CONVOCATORIA CAS";
            string nombrePuesto = "PLAZA SELECCIONADA";

            if (primerCandidato != null)
            {
                var client = _httpClientFactory.CreateClient("SistemaPublicacionConvocatorias");
                try
                {
                    var plazaJava = await client.GetFromJsonAsync<PlazaJavaDto>($"convocatorias/{request.IdPlaza}", cancellationToken);
                    if (plazaJava != null)
                    {
                        codigoConvocatoria = plazaJava.CodigoConvocatoria;
                        nombrePuesto = plazaJava.Cargo.NombreCargo;
                    }
                }
                catch { /* Fail-safe */ }
            }

            return await _comiteService.ObtenerActaFinalConsolidadaPdfAsync(candidatos, codigoConvocatoria.ToUpper(), nombrePuesto.ToUpper());
        }
    }
}
