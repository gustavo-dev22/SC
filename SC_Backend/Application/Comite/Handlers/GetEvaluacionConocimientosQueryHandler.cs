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
    public class GetEvaluacionConocimientosQueryHandler : IRequestHandler<GetEvaluacionConocimientosQuery, List<EvaluacionConocimientosDto>>
    {
        private readonly IComiteEvaluadorService _comiteService;
        private readonly IHttpClientFactory _httpClientFactory;

        public GetEvaluacionConocimientosQueryHandler(IComiteEvaluadorService comiteService, IHttpClientFactory httpClientFactory)
        {
            _comiteService = comiteService;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<List<EvaluacionConocimientosDto>> Handle(GetEvaluacionConocimientosQuery request, CancellationToken cancellationToken)
        {
            // 1. Extraemos los candidatos aptos para examen desde SQL Server via Dapper
            var candidatos = await _comiteService.ListarEvaluacionConocimientosAsync(request.IdPlaza);

            if (candidatos.Count > 0)
            {
                // 2. Conectamos con el HttpClient de Java Spring Boot
                var client = _httpClientFactory.CreateClient("SistemaPublicacionConvocatorias");

                // Variables temporales para el fail-safe
                string codigoConvocatoria = "ERR-KV";
                string nombrePuesto = "Plaza Temporalmente No Disponible";

                try
                {
                    // Al ser la misma plaza para todos, consultamos a Java UNA SOLA VEZ fuera del bucle 🚀
                    var plazaJava = await client.GetFromJsonAsync<PlazaJavaDto>($"convocatorias/{request.IdPlaza}", cancellationToken);
                    if (plazaJava != null)
                    {
                        codigoConvocatoria = plazaJava.CodigoConvocatoria;
                        nombrePuesto = plazaJava.Cargo.NombreCargo;
                    }
                }
                catch
                {
                    // Si el microservicio de Java cae, se conservan los valores del fail-safe de arriba
                }

                // 3. Inyectamos los nombres de la plaza y puesto recuperados a todos los candidatos
                foreach (var cand in candidatos)
                {
                    cand.CodigoConvocatoria = codigoConvocatoria;
                    cand.NombrePuesto = nombrePuesto;
                }
            }

            return candidatos;
        }
    }
}
