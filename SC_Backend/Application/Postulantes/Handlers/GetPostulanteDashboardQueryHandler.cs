using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Oportunidades.Dtos;
using Application.Postulantes.Dtos;
using Application.Postulantes.Queries;
using MediatR;

namespace Application.Postulantes.Handlers
{
    public class GetPostulanteDashboardQueryHandler : IRequestHandler<GetPostulanteDashboardQuery, List<PostulanteDashboardDto>>
    {
        private readonly IPostulanteQueryService _postulanteService;
        private readonly IHttpClientFactory _httpClientFactory;

        public GetPostulanteDashboardQueryHandler(IPostulanteQueryService postulanteService, IHttpClientFactory httpClientFactory)
        {
            _postulanteService = postulanteService;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<List<PostulanteDashboardDto>> Handle(GetPostulanteDashboardQuery request, CancellationToken cancellationToken)
        {
            var postulaciones = await _postulanteService.ObtenerDashboardPostulanteAsync(request.IdPostulante);

            if (postulaciones.Count > 0)
            {
                var client = _httpClientFactory.CreateClient("SistemaPublicacionConvocatorias");
                foreach (var post in postulaciones)
                {
                    try
                    {
                        var plaza = await client.GetFromJsonAsync<PlazaJavaDto>($"convocatorias/{post.IdPlaza}");
                        if (plaza != null)
                        {
                            post.CodigoConvocatoria = plaza.CodigoConvocatoria;
                            post.NombrePuesto = plaza.Cargo.NombreCargo;
                        }
                    }
                    catch { /* Fail-safe si Java no responde */ }
                }
            }
            return postulaciones;
        }
    }
}
