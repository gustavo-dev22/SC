using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Application.Comite.Dtos;
using Application.Comite.Queries;
using Application.Common.Interfaces;
using Application.Oportunidades.Dtos;
using MediatR;

namespace Application.Comite.Handlers
{
    public class GetEvaluacionEntrevistaQueryHandler : IRequestHandler<GetEvaluacionEntrevistaQuery, List<EvaluacionEntrevistaDto>>
    {
        private readonly IComiteEvaluadorService _comiteService;
        private readonly IHttpClientFactory _httpClientFactory;

        public GetEvaluacionEntrevistaQueryHandler(IComiteEvaluadorService comiteService, IHttpClientFactory httpClientFactory)
        {
            _comiteService = comiteService;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<List<EvaluacionEntrevistaDto>> Handle(GetEvaluacionEntrevistaQuery request, CancellationToken cancellationToken)
        {
            var candidatos = await _comiteService.ListarCandidatosEntrevistaAsync(request.IdPlaza);
            if (candidatos.Count > 0)
            {
                var client = _httpClientFactory.CreateClient("SistemaPublicacionConvocatorias");
                try
                {
                    var plazaJava = await client.GetFromJsonAsync<PlazaJavaDto>($"convocatorias/{request.IdPlaza}", cancellationToken);
                    if (plazaJava != null)
                    {
                        foreach (var cand in candidatos)
                        {
                            cand.CodigoConvocatoria = plazaJava.CodigoConvocatoria;
                            cand.NombrePuesto = plazaJava.Cargo.NombreCargo;
                        }
                    }
                }
                catch { /* Fail-safe */ }
            }
            return candidatos;
        }
    }
}
