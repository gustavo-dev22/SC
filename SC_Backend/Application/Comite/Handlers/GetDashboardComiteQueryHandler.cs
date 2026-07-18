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
    public class GetDashboardComiteQueryHandler : IRequestHandler<GetDashboardComiteQuery, ComiteDashboardDto>
    {
        private readonly IComiteEvaluadorService _comiteService;
        private readonly IHttpClientFactory _httpClientFactory;

        public GetDashboardComiteQueryHandler(IComiteEvaluadorService comiteService, IHttpClientFactory httpClientFactory)
        {
            _comiteService = comiteService;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<ComiteDashboardDto> Handle(GetDashboardComiteQuery request, CancellationToken cancellationToken)
        {
            // 1. Extraemos la metadata transaccional de candidatos y tickets desde SQL Server
            var dashboardDto = await _comiteService.ObtenerDashboardComiteAsync(request.NombreUsuario);

            // 2. Enriquecemos el total de plazas consultando dinámicamente al SPC (Java)
            var client = _httpClientFactory.CreateClient("SistemaPublicacionConvocatorias");
            try
            {
                // Consumimos el endpoint base que lista las plazas vigentes o asignadas del concurso
                // Ajusta la ruta exacta de tu API de Java (ej. "convocatorias" o "convocatorias/vigentes")
                var plazasJava = await client.GetFromJsonAsync<List<PlazaJavaDto>>("convocatorias", cancellationToken);

                if (plazasJava != null)
                {
                    // Seteamos de forma dinámica el conteo real del microservicio externo
                    dashboardDto.Metricas.TotalPlazasAsignadas = plazasJava.Count;
                }
            }
            catch
            {
                // Fail-safe institucional: Si el microservicio de Java se cae temporalmente, 
                // colocamos un valor de resguardo por defecto para no romper el dashboard
                dashboardDto.Metricas.TotalPlazasAsignadas = 0;
            }

            return dashboardDto;
        }
    }
}
