using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Application.Admin.Dtos;
using Application.Admin.Queries;
using Application.Common.Interfaces;
using Application.Oportunidades.Dtos;
using MediatR;

namespace Application.Admin.Handlers
{
    public class GetTrazabilidadQueryHandler : IRequestHandler<GetTrazabilidadQuery, List<TrazabilidadExpedienteDto>>
    {
        private readonly IAdminPostulacionService _adminService;
        private readonly IHttpClientFactory _httpClientFactory;

        public GetTrazabilidadQueryHandler(IAdminPostulacionService adminService, IHttpClientFactory httpClientFactory)
        {
            _adminService = adminService;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<List<TrazabilidadExpedienteDto>> Handle(GetTrazabilidadQuery request, CancellationToken cancellationToken)
        {
            var lineasTiempo = await _adminService.ObtenerTrazabilidadPorExpedienteAsync(request.CodigoExpediente);

            if (lineasTiempo.Count > 0)
            {
                var primerHito = lineasTiempo.First();
                var client = _httpClientFactory.CreateClient("SistemaPublicacionConvocatorias");

                try
                {
                    // Un solo viaje a Java para rellenar los datos de la Plaza de todo el set
                    var plaza = await client.GetFromJsonAsync<PlazaJavaDto>($"convocatorias/{primerHito.IdPlaza}");
                    if (plaza != null)
                    {
                        lineasTiempo.ForEach(h => {
                            h.CodigoConvocatoria = plaza.CodigoConvocatoria;
                            h.NombrePuesto = plaza.Cargo.NombreCargo;
                        });
                    }
                }
                catch { /* Fail-safe */ }
            }

            return lineasTiempo;
        }
    }
}
