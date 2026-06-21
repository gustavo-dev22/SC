using Application.Postulantes.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostulantePdfReporteController : ControllerBase
    {
        private readonly IMediator _mediator;
        public PostulantePdfReporteController(IMediator mediator) => _mediator = mediator;

        [HttpGet("reporte-ficha/{idPostulante:int}")]
        [ProducesResponseType(typeof(FileResult), 200)]
        public async Task<IActionResult> ImprimirFichaPdf(int idPostulante)
        {
            byte[] pdfBytes = await _mediator.Send(new ObtenerFichaPdfQuery(idPostulante));

            string nombreArchivo = $"FICHA_POSTULANTE_{idPostulante:D6}.pdf";

            // Retornamos el File Content Result nativo de ASP.NET Core
            return File(pdfBytes, "application/pdf", nombreArchivo);
        }

        [HttpGet("reporte-constancia/{idPostulacion:int}")]
        [ProducesResponseType(typeof(FileResult), 200)]
        public async Task<IActionResult> ImprimirConstanciaPdf(int idPostulacion)
        {
            // Enviamos la Query por el bus de MediatR
            byte[] pdfBytes = await _mediator.Send(new ObtenerConstanciaPdfQuery(idPostulacion));

            string nombreArchivo = $"CONSTANCIA_POSTULACION_{idPostulacion:D6}.pdf";

            // Retornamos el File Content Result nativo de ASP.NET Core
            return File(pdfBytes, "application/pdf", nombreArchivo);
        }
    }
}
