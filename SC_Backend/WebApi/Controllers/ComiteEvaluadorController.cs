using Application.Admin.Commands;
using Application.Admin.Dtos;
using Application.Comite.Commands;
using Application.Comite.Queries;
using Application.Common.Dtos;
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
            var candidatos = await _mediator.Send(new GetEvaluacionConocimientosQuery(idPlaza));
            string nombrePuesto = candidatos.FirstOrDefault()?.PostulanteNombre != null ? "EVALUACION_CONOCIMIENTOS" : "ACTA";
            return File(pdfBytes, "application/pdf", $"Acta_Conocimientos_Plaza_{idPlaza}.pdf");
        }

        [HttpGet("listar-inscritos/{idPlaza}")]
        public async Task<IActionResult> GetCalificacionCurricular(int idPlaza)
        {
            var data = await _mediator.Send(new GetCalificacionCurricularQuery(idPlaza));
            return Ok(new { success = true, data });
        }

        [HttpPost("guardar-calificacion")]
        public async Task<IActionResult> GuardarCalificacionCurricular([FromBody] GuardarCalificacionCurricularCommand command)
        {
            var res = await _mediator.Send(command);
            return Ok(new { success = res });
        }

        [HttpGet("exportar-acta-curricular/{idPlaza}")]
        public async Task<IActionResult> ExportarActaCurricular(int idPlaza)
        {
            var pdfBytes = await _mediator.Send(new GetActaCurricularPdfQuery(idPlaza));
            return File(pdfBytes, "application/pdf", $"Acta_Evaluacion_Curricular_Plaza_{idPlaza}.pdf");
        }

        [HttpGet("entrevista-personal")]
        public async Task<IActionResult> GetEntrevistaPersonal([FromQuery] int idPlaza)
        {
            var data = await _mediator.Send(new GetEvaluacionEntrevistaQuery(idPlaza));
            return Ok(new { success = true, data });
        }

        [HttpPost("registrar-nota-entrevista")]
        public async Task<IActionResult> RegistrarNotaEntrevista([FromBody] RegistrarNotaEntrevistaCommand command)
        {
            var res = await _mediator.Send(command);
            return Ok(new { success = res });
        }

        [HttpGet("exportar-acta-entrevista/{idPlaza}")]
        public async Task<IActionResult> ExportarActaEntrevista(int idPlaza)
        {
            var pdfBytes = await _mediator.Send(new GetActaEntrevistaPdfQuery(idPlaza));
            return File(pdfBytes, "application/pdf", $"Acta_Resultados_Finales_Plaza_{idPlaza}.pdf");
        }

        [HttpGet("cuadro-merito-final")]
        public async Task<IActionResult> GetCuadroMeritoFinal([FromQuery] int idPlaza)
        {
            var data = await _mediator.Send(new GetCuadroMeritoFinalQuery(idPlaza));
            return Ok(new { success = true, data });
        }

        [HttpGet("exportar-acta-final/{idPlaza}")]
        public async Task<IActionResult> ExportarActaFinal(int idPlaza)
        {
            var pdfBytes = await _mediator.Send(new GetActaFinalPdfQuery(idPlaza));
            return File(pdfBytes, "application/pdf", $"Acta_Resultados_Finales_Plaza_{idPlaza}.pdf");
        }

        [HttpGet("consultas-tecnicas")]
        public async Task<IActionResult> GetConsultasTecnicas([FromQuery] int? idEstado, [FromQuery] string? busqueda)
        {
            var result = await _mediator.Send(new GetConsultasTecnicasComiteQuery(idEstado, busqueda));
            return Ok(BaseResponse<List<AdminTicketBandejaDto>>.Ok(result, "Consultas técnicas cargadas correctamente."));
        }

        [HttpPost("atender-consulta")]
        public async Task<IActionResult> AtenderConsulta([FromBody] AtenderTicketCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(BaseResponse<bool>.Ok(result, "Consulta técnica procesada de manera conforme."));
        }
    }
}
