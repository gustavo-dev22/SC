using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Postulantes.Queries;
using MediatR;

namespace Application.Postulantes.Handlers
{
    public class ObtenerConstanciaPdfQueryHandler : IRequestHandler<ObtenerConstanciaPdfQuery, byte[]>
    {
        private readonly IOportunidadesQueryService _oportunidadesService;
        private readonly IPostulanteQueryService _postulanteService;

        // 🚀 Inyectamos solo los dos QueryServices necesarios
        public ObtenerConstanciaPdfQueryHandler(
            IOportunidadesQueryService oportunidadesService,
            IPostulanteQueryService postulanteService)
        {
            _oportunidadesService = oportunidadesService;
            _postulanteService = postulanteService;
        }

        public async Task<byte[]> Handle(ObtenerConstanciaPdfQuery request, CancellationToken cancellationToken)
        {
            // 1. Buscamos los datos cruzados de la postulación (Convocatoria, puesto de Java, etc.)
            // Nota: Asegúrate de que este método exista en tu OportunidadesQueryService
            var datosPostulacion = await _oportunidadesService.ObtenerPostulacionPorIdAsync(request.IdPostulacion);
            if (datosPostulacion == null)
                throw new KeyNotFoundException("No se encontró el registro de la postulación.");

            // 2. Extraemos los bytes de la firma digital del postulante
            byte[]? firmaBytes = await _postulanteService.ObtenerFirmaBytesAsync(datosPostulacion.IdPostulante);

            // 3. 🚀 LLAMADA CORREGIDA: Invocamos tu método de QuestPDF dentro de PostulanteQueryService
            return await _postulanteService.ObtenerConstanciaPostulacionPdfAsync(datosPostulacion, firmaBytes);
        }
    }
}
