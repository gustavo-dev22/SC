using Application.Comite.Dtos;
using Application.Comite.Queries;
using Application.Common.Interfaces;
using Application.Oportunidades.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Application.Comite.Handlers
{
    public class GetCalificacionCurricularQueryHandler : IRequestHandler<GetCalificacionCurricularQuery, List<CalificacionCurricularDto>>
    {
        private readonly IComiteEvaluadorService _comiteService;
        private readonly IHttpClientFactory _httpClientFactory;

        public GetCalificacionCurricularQueryHandler(IComiteEvaluadorService comiteService, IHttpClientFactory httpClientFactory)
        {
            _comiteService = comiteService;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<List<CalificacionCurricularDto>> Handle(GetCalificacionCurricularQuery request, CancellationToken cancellationToken)
        {
            var candidatos = await _comiteService.ListarCandidatosCurricularAsync(request.IdPlaza);

            if (candidatos.Count > 0)
            {
                var client = _httpClientFactory.CreateClient("SistemaPublicacionConvocatorias");
                string codigoConvocatoria = "CONVOCATORIA CAS";
                string nombrePuesto = "PLAZA EN EVALUACIÓN";

                try
                {
                    var plazaJava = await client.GetFromJsonAsync<PlazaJavaDto>($"convocatorias/{request.IdPlaza}", cancellationToken);
                    if (plazaJava != null)
                    {
                        codigoConvocatoria = plazaJava.CodigoConvocatoria;
                        nombrePuesto = plazaJava.Cargo.NombreCargo;
                    }
                }
                catch { /* Fail-safe activado */ }

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
