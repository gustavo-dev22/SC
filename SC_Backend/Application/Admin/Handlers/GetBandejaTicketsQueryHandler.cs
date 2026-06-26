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
    public class GetBandejaTicketsQueryHandler : IRequestHandler<GetBandejaTicketsQuery, List<AdminTicketBandejaDto>>
    {
        private readonly IAdminSoporteService _adminService;
        private readonly IHttpClientFactory _httpClientFactory;

        public GetBandejaTicketsQueryHandler(IAdminSoporteService adminService, IHttpClientFactory httpClientFactory)
        {
            _adminService = adminService;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<List<AdminTicketBandejaDto>> Handle(GetBandejaTicketsQuery request, CancellationToken cancellationToken)
        {
            var tickets = await _adminService.ObtenerBandejaTicketsAsync(request.IdEstado, request.Busqueda);
            var client = _httpClientFactory.CreateClient("SistemaPublicacionConvocatorias");

            // Cruzamos en caliente con Spring Boot (Java) si el ticket está enlazado a una plaza
            foreach (var t in tickets)
            {
                if (t.IdPlaza.HasValue && t.IdPlaza.Value > 0)
                {
                    try
                    {
                        var plaza = await client.GetFromJsonAsync<PlazaJavaDto>($"convocatorias/{t.IdPlaza}");
                        if (plaza != null) t.CodigoConvocatoria = plaza.CodigoConvocatoria;
                    }
                    catch { t.CodigoConvocatoria = "CONV-ERR"; }
                }
            }
            return tickets;
        }
    }
}
