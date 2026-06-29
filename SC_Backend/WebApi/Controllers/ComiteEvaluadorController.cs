using Application.Comite.Commands;
using Application.Comite.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComiteEvaluadorController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ComiteEvaluadorController(IMediator mediator) => _mediator = mediator;

        [HttpGet("expedientes-inscritos")]
        public async Task<IActionResult> GetExpedientes([FromQuery] int? idPlaza)
        {
            var data = await _mediator.Send(new GetExpedientesInscritosQuery(idPlaza));
            return Ok(new { success = true, data });
        }

        [HttpPost("evaluar-inicial")]
        public async Task<IActionResult> EvaluarInicial([FromBody] EvaluarExpedienteCommand command)
        {
            var res = await _mediator.Send(command);
            return Ok(new { success = res });
        }

        [HttpGet("exportar-acta-inicial/{idPlaza}")]
        public async Task<IActionResult> ExportarActaInicial(int idPlaza)
        {
            var pdfBytes = await _mediator.Send(new GetActaInicialPdfQuery(idPlaza));

            var expedientes = await _mediator.Send(new GetExpedientesInscritosQuery(idPlaza));
            string nombrePuesto = expedientes.FirstOrDefault()?.NombrePuesto ?? "Plaza_Seleccionada";

            string nombrePuestoLimpio = nombrePuesto.Replace(" ", "_");

            string nombreArchivo = $"Acta_Filtro_Inicial_Plaza_{nombrePuestoLimpio}.pdf";

            return File(pdfBytes, "application/pdf", nombreArchivo);
        }

        [HttpGet("examen-conocimientos")]
        public async Task<IActionResult> GetExamenConocimientos([FromQuery] int idPlaza)
        {
            var data = await _mediator.Send(new GetEvaluacionConocimientosQuery(idPlaza));
            return Ok(new { success = true, data });
        }

        [HttpPost("registrar-nota-examen")]
        public async Task<IActionResult> RegistrarNotaExamen([FromBody] RegistrarNotaConocimientosCommand command)
        {
            var res = await _mediator.Send(command);
            return Ok(new { success = res });
        }

        [HttpGet("exportar-acta-conocimientos/{idPlaza}")]
        public async Task<IActionResult> ExportarActaConocimientos(int idPlaza)
        {
            var pdfBytes = await _mediator.Send(new GetActaConocimientosPdfQuery(idPlaza));
            // Recuperamos el listado rápido para nombrar el archivo
            var candidatos = await _mediator.Send(new GetEvaluacionConocimientosQuery(idPlaza));
            string nombrePuesto = candidatos.FirstOrDefault()?.PostulanteNombre != null ? "EVALUACION_CONOCIMIENTOS" : "ACTA";
            // Puedes inyectarle el nombre real mapeado de Java
            return File(pdfBytes, "application/pdf", $"Acta_Conocimientos_Plaza_{idPlaza}.pdf");
        }
    }
}
