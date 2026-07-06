using Application.Common.Dtos;
using Application.Postulantes.Commands;
using Application.Postulantes.Dtos;
using Application.Postulantes.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostulanteCertificacionController : ControllerBase
    {
        private readonly IMediator _mediator;
        public PostulanteCertificacionController(IMediator mediator) => _mediator = mediator;

        [HttpGet("postulante/{idPostulante}")]
        public async Task<IActionResult> Get(int idPostulante)
        {
            var result = await _mediator.Send(new GetCertificacionesQuery(idPostulante));
            return Ok(BaseResponse<List<PostulanteCertificacionDto>>.Ok(result));
        }

        [HttpPost("mantenimiento")]
        public async Task<IActionResult> Mantenimiento([FromBody] MantenimientoCertificacionCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(BaseResponse<bool>.Ok(result, "Certificación procesada con éxito."));
        }

        [HttpPost("subir-sustento")]
        public async Task<IActionResult> SubirSustento([FromForm] int idCertificacion, [FromForm] IFormFile archivo)
        {
            // Reutilizamos el mismo comando universal pero cambiando a "CERTIFICACION"
            var command = new SubirSustentoCommand(idCertificacion, archivo, "CERTIFICACION");
            var resultado = await _mediator.Send(command);

            if (resultado) return Ok(new { success = true, message = "Archivo sustentatorio de certificación guardado correctamente." });
            return BadRequest(new { success = false, message = "No se pudo procesar el archivo cargado." });
        }

        [HttpDelete("eliminar-sustento/{idCertificacion}")]
        public async Task<IActionResult> EliminarSustento(int idCertificacion)
        {
            var resultado = await _mediator.Send(new EliminarSustentoCommand(idCertificacion, "CERTIFICACION"));
            if (resultado) return Ok(new { success = true, message = "Sustento de certificación eliminado correctamente." });
            return BadRequest(new { success = false, message = "No se pudo eliminar el sustento." });
        }
    }
}
