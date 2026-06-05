using Application.Catalogos.Dtos;
using Application.Catalogos.Queries;
using Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Application.Catalogos.Handlers
{
    public class GetCentrosEstudiosQueryHandler : IRequestHandler<GetCentrosEstudiosQuery, List<CentroEstudioDto>>
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ICatalogoQueryService _catalogoQueryService;

        public GetCentrosEstudiosQueryHandler(IHttpClientFactory httpClientFactory, IConfiguration configuration, ICatalogoQueryService catalogoQueryService)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _catalogoQueryService = catalogoQueryService;
        }

        public async Task<List<CentroEstudioDto>> Handle(GetCentrosEstudiosQuery request, CancellationToken cancellationToken)
        {
            var resultadoUnificado = new List<CentroEstudioDto>();
            string filtroLimpio = (request.Filtro ?? "").Trim().ToUpper();

            // 1. TAREA A: Obtener Universidades desde la API (Proxy)
            var tareaUniversidades = ObtenerUniversidadesApiAsync(filtroLimpio, cancellationToken);

            // 2. TAREA B: Obtener Institutos desde el Query Service de la capa de Persistencia
            var tareaInstitutos = _catalogoQueryService.ListarInstitutosPredictivoAsync(filtroLimpio);

            // 3. TAREA C: Obtener Entidades Públicas desde el Query Service de la capa de Persistencia
            var tareaEntidades = _catalogoQueryService.ListarEntidadesPublicasPredictivoAsync(filtroLimpio);

            // Ejecución en paralelo de alto rendimiento (Zoneless compliant)
            await Task.WhenAll(tareaUniversidades, tareaInstitutos, tareaEntidades);

            // 3. Consolidación limpia
            resultadoUnificado.AddRange(tareaUniversidades.Result);
            resultadoUnificado.AddRange(tareaInstitutos.Result);
            resultadoUnificado.AddRange(tareaEntidades.Result);

            return resultadoUnificado.OrderBy(x => x.Nombre).ToList();
        }

        private async Task<List<CentroEstudioDto>> ObtenerUniversidadesApiAsync(string filtro, CancellationToken cancellationToken)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                string url = _configuration["IntegrationServices:UniversitiesApiUrl"];

                var response = await client.GetFromJsonAsync<List<JsonUniversidadItem>>(url, cancellationToken);

                if (response == null) return new List<CentroEstudioDto>();

                return response
                    .Where(u => string.IsNullOrEmpty(filtro) || u.Name.ToUpper().Contains(filtro))
                    .Select(u => new CentroEstudioDto
                    {
                        Nombre = u.Name.ToUpper(),
                        TipoProvider = "UNIVERSIDAD"
                    }).ToList();
            }
            catch
            {
                return new List<CentroEstudioDto>();
            }
        }

        private class JsonUniversidadItem { public string Name { get; set; } = string.Empty; }
    }
}
