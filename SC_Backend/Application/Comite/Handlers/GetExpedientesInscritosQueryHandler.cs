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
    public class GetExpedientesInscritosQueryHandler : IRequestHandler<GetExpedientesInscritosQuery, List<ExpedienteInscritoDto>>
    {
        private readonly IComiteEvaluadorService _comiteService;
        private readonly IHttpClientFactory _httpClientFactory;

        public GetExpedientesInscritosQueryHandler(IComiteEvaluadorService comiteService, IHttpClientFactory httpClientFactory)
        {
            _comiteService = comiteService;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<List<ExpedienteInscritoDto>> Handle(GetExpedientesInscritosQuery request, CancellationToken cancellationToken)
        {
            // 1. Extraemos los expedientes en estado 1201 desde SQL Server via Dapper
            var expedientes = await _comiteService.ListarExpedientesInscritosAsync(request.IdPlaza);

            if (expedientes.Count > 0)
            {
                // 2. Conectamos con el HttpClient de Java Spring Boot para inyectar los nombres de los puestos
                var client = _httpClientFactory.CreateClient("SistemaPublicacionConvocatorias");

                foreach (var exp in expedientes)
                {
                    try
                    {
                        var plazaJava = await client.GetFromJsonAsync<PlazaJavaDto>($"convocatorias/{exp.IdPlaza}", cancellationToken);
                        if (plazaJava != null)
                        {
                            exp.CodigoConvocatoria = plazaJava.CodigoConvocatoria;
                            exp.NombrePuesto = plazaJava.Cargo.NombreCargo;
                        }
                    }
                    catch
                    {
                        // Fail-safe: Si el microservicio de Java no responde, el flujo no se interrumpe
                        exp.CodigoConvocatoria = "ERR-KV";
                        exp.NombrePuesto = "Plaza Temporalmente No Disponible";
                    }
                }
            }

            return expedientes;
        }
    }
}
